using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public Vector2 playerAxis;
    public float playerVelocity;
    public float jumpForce;
    public float fallMultiply;
    public float meleeAttackRange;
    public float attackSpeed;
    public float attackFireRate;
    public float projectileSpeed;
    public float stepDistance;
    public float stepStopTime;
    public float comboID = 1;
    public float resetComboTime;
    public float currentComboTime;
    public float minWaterBallSize;
    public float maxWaterBallSize;
    public float currentWaterBallSize;
    public float waterBallIncreaseValue;
    public float waterSpendValue;

    public bool facingRight;
    public bool showGizmos;
    public bool isAttacking;
    public bool jumping;
    public bool countComboResetTime;
    public bool charging;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform dustPosition;
    public Transform stopPosition;
    public Rigidbody2D rb;
    public Animator anim;
    public Slider waterSlider;

    public GameObject waterBall;
    public GameObject waterProjectile;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        waterSlider.minValue = minWaterBallSize;
        waterSlider.maxValue = maxWaterBallSize;
    }

    public ParticleSystem dust;


    void Start() {
        
    }

    void Update() {
        rb.velocity = new Vector2(playerAxis.x * playerVelocity, rb.velocity.y);

        if(countComboResetTime) {
            currentComboTime += Time.deltaTime;
            if(currentComboTime > resetComboTime) {
                currentComboTime = 0;
                comboID = 1;
                countComboResetTime = false;
            }
        }

        if (charging) {
            if(currentWaterBallSize < minWaterBallSize) currentWaterBallSize = minWaterBallSize;
            if (currentWaterBallSize < maxWaterBallSize)
                currentWaterBallSize += waterBallIncreaseValue * Time.deltaTime;
            else currentWaterBallSize = maxWaterBallSize;
            waterBall.transform.localScale = Vector2.one * currentWaterBallSize;
        }

        waterSlider.value = currentWaterBallSize;

        PassAnimationValues();

        TryResetCombo();
        TryFlip();

        

        

    }

    private void TryResetCombo() {
        if (countComboResetTime) {
            currentComboTime += Time.deltaTime;
            if (currentComboTime > resetComboTime) {
                currentComboTime = 0;
                comboID = 0;
                countComboResetTime = false;
            }
        }
    }

    private void PassAnimationValues() {
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(rb.velocity.x));
        
        int verticalSpeed = rb.velocity.y >= 0 ? 1 : 0 ;
        anim.SetFloat("VerticalSpeed", Mathf.Abs(verticalSpeed));
        anim.SetBool("IsGrounded", IsGrounded());
        anim.SetBool("Jumping", jumping);
    }

    private void TryFlip() {
        if (rb.velocity.x > 0 && !facingRight) Flip();
        else if (rb.velocity.x < 0 && facingRight) Flip();
    }

    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
        
    }

    public void PlayerMove(InputAction.CallbackContext context) {
        if (!isAttacking) {
            playerAxis = context.ReadValue<Vector2>();
            if (context.canceled && playerAxis.x == 0) {
                anim.SetTrigger("Stop");
                if (IsGrounded()) SpawnStopParticle();
            }
        }
    }

    public void Jump(InputAction.CallbackContext context) {
        if (isAttacking) return;
        if (context.performed && IsGrounded()) {
            jumping = true;
            anim.SetTrigger("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.gravityScale = 4;
            SpawnParticles.instance.SpawnParticle(SpawnParticles.instance.newParticle("Dust"), dustPosition.position);
        } else if (context.canceled && rb.velocity.y > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * fallMultiply);
            rb.gravityScale = 2;
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed) {

        }
    }

    public void Attack(InputAction.CallbackContext context) {
        if (isAttacking || !IsGrounded()) return;
        if (currentWaterBallSize < minWaterBallSize + waterSpendValue) return;
        playerAxis.x = 0;
        if (context.performed) {
            var mousePosition = Mouse.current.position.ReadValue();
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var distance = (Vector2)worldMousePosition - (Vector2)transform.position;
            float dir = distance.x >= 0 ? 1 : -1;
            if (distance.magnitude <= meleeAttackRange) MeleeAttack(dir);
            else RangedAttack();
        }
    }

    private void MeleeAttack(float dir) {
        Debug.Log("Melee Attack");
        countComboResetTime = true;
        currentComboTime = 0;
        if (comboID == 3) comboID = 1;
        else comboID++;
        isAttacking = true;
        anim.SetFloat("ComboID", comboID);
        anim.SetTrigger("MeleeAttack");
        StartCoroutine(MeleeWaterballAttack(dir));
    }

    private IEnumerator MeleeWaterballAttack(float dir) {
        SpendWater();
        TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
        PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
        trail.enabled = true;
        DOTween.To(() => ball.weight, (v) => ball.weight = v, 1, attackSpeed);
        playerAxis.x = dir;
        yield return new WaitForSeconds(attackSpeed);
        playerAxis.x = 0;
        yield return new WaitForSeconds(attackFireRate);
        isAttacking = false;
    }

    public void DisableBallFollow() {
        TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
        PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
        DOTween.To(() => ball.weight, (v) => ball.weight = v, 0, attackSpeed);
        trail.enabled = false;
    }

    public void RangedAttack() {
        Debug.Log("Ranged Attack");
        isAttacking = true;
        anim.SetTrigger("RangedAttack");
    }

    public void UseRangedAttack() {
        SpendWater();
        GameObject newProjectile = Instantiate(waterProjectile, waterBall.transform.position, Quaternion.identity);
        newProjectile.transform.localScale = newProjectile.transform.localScale * currentWaterBallSize;

        var mousePosition = Mouse.current.position.ReadValue();
        var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        var distance = (Vector2)worldMousePosition - (Vector2)transform.position;
        var direction = (Vector2)worldMousePosition - (Vector2)waterBall.transform.position;

        newProjectile.GetComponent<Rigidbody2D>().velocity = (direction.normalized * projectileSpeed);
        newProjectile = null;

        HorizontalImpulse(distance);
    }

    private void HorizontalImpulse(Vector3 mouseDistance) {
        float dir = mouseDistance.x >= 0 ? 1 : -1;
        if (dir > 0 && !facingRight) Flip();
        else if (dir < 0 && facingRight) Flip();

        //StartCoroutine(Impulse(stepDistance * dir));
    }

    IEnumerator Impulse(float dir) {
        
        playerAxis.x = dir;
        yield return new WaitForSeconds(stepStopTime);
        playerAxis.x = 0;
        
    }

    public void DisableAttack() {
        isAttacking = false;
        var input = GetComponent<PlayerInput>();
        //input.
    }

    public void SpecialAttack(InputAction.CallbackContext context) {
        if (!IsGrounded()) return;

        if (context.performed && !isAttacking) {
            if (!charging) {
                charging = true;
                isAttacking = true;
                waterBall.GetComponent<WaterDropsGenerator>().StartSpawn();
                anim.SetBool("Charging", charging);
                TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
                PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
                trail.enabled = true;
                DOTween.To(() => ball.weight, (v) => ball.weight = v, 1, attackSpeed);
            }
        }

        if (context.canceled) {
            if (charging) {
                charging = false;
                isAttacking = false;
                waterBall.GetComponent<WaterDropsGenerator>().StopAllCoroutines();
                anim.SetBool("Charging", charging);
                TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
                PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
                //trail.enabled = false;
                DOTween.To(() => ball.weight, (v) => ball.weight = v, 0, attackSpeed);
            }
        }
    }

    public void SpendWater() {
        if (currentWaterBallSize - waterSpendValue >= minWaterBallSize)
            currentWaterBallSize -= waterSpendValue;
        else if (currentWaterBallSize < minWaterBallSize)
            currentWaterBallSize = minWaterBallSize;
        waterBall.transform.localScale = Vector2.one * currentWaterBallSize;
    }

    public void SpawnStopParticle() {
        SpawnParticles.instance.SpawnParticle(SpawnParticles.instance.newParticle("Stop"), stopPosition.position, transform.localScale.x);
    }

    public void SpawnJumpParticle() {
        SpawnParticles.instance.SpawnParticle(SpawnParticles.instance.newParticle("Dust2"), dustPosition.position, transform.localScale.x);
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground") {
            if (Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer)) {
                jumping = false;
                SpawnJumpParticle();
            }
        }
    }

    private void OnDrawGizmos() {
        if (showGizmos == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

}