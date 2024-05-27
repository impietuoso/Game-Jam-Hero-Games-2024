using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Player : MonoBehaviour {

    #region Variaveis

    public static Player instance;
    private Vector2 playerAxis;
    public float playerVelocity;
    public float jumpForce;
    public float fallMultiply;
    private float walkValue;
    public float resetWalkValue;
    public float jumpingSlow;

    [Space(25)]
    public float meleeAttackRange;
    public float attackSpeed;
    public float attackFireRate;
    public float projectileSpeed;
    public float stepDistance;
    public float stepStopTime;
    private float comboID = 1;
    public float resetComboTime;
    public float currentComboTime;
    public float minWaterBallSize;
    public float maxWaterBallSize;
    private float currentWaterBallSize;
    public float waterBallIncreaseValue;
    public float waterSpendValue;

    [Space(25)]
    public int mouseCursor;
    public int maxPlayerHp;
    private int currentPlayerHp;
    public int playerDamage;

    [Space(25)]
    public bool facingRight;
    private bool isAttacking;
    private bool jumping;
    private bool countComboResetTime;
    private bool charging;
    private bool isStoping;
    private bool canCountWalkValue;
    public bool showGizmos;
    private bool dead;

    [Space(25)]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform dustPosition;
    public Transform stopPosition;
    private Rigidbody2D rb;
    private Animator anim;
    public ParticleSystem walkDustParticle;
    public Playlist playlist;
    public Slider waterSlider;
    public Slider hpSlider;

    [Space(25)]
    public GameObject waterBall;
    public GameObject waterProjectile;

    private PlayerInput input;
    private InputAction.CallbackContext lastInput;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    #endregion

    void Awake() {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playlist = GetComponent<Playlist>();
        input = GetComponent<PlayerInput>();
        waterSlider.minValue = minWaterBallSize;
        waterSlider.maxValue = maxWaterBallSize;
        hpSlider.maxValue = maxPlayerHp;
    }

    void Start() {
        playerActionMap = input.actions.FindActionMap("Keyboard");
        uiActionMap = input.actions.FindActionMap("UI");
        currentPlayerHp = maxPlayerHp;
        hpSlider.value = currentPlayerHp;
    }

    void Update() {
        if (dead) return;
        if (IsGrounded()) rb.velocity = new Vector2(playerAxis.x * playerVelocity, rb.velocity.y);
        else rb.velocity = new Vector2(playerAxis.x * (playerVelocity - jumpingSlow), rb.velocity.y);

        if (countComboResetTime) {
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

        if (playerAxis.x != 0 && canCountWalkValue) walkValue += Mathf.Abs(playerAxis.x) * Time.deltaTime;

        waterSlider.value = currentWaterBallSize;

        PassAnimationValues();

        TryResetCombo();
        TryFlip();
    }

    private void LateUpdate() {
        SwapCursor();
    }

    #region Controles

    public void EnablePlayerControls() {
        uiActionMap.Disable();
        playerActionMap.Enable();
    }

    public void EnableUIControls() {
        Debug.Log("Disable Player Actions");
        playerActionMap.Disable();
        uiActionMap.Enable();
    }

    #endregion

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
        if (dead) return;
        if (rb.velocity.x > 0 && !facingRight) Flip();
        else if (rb.velocity.x < 0 && facingRight) Flip();
    }

    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
        
    }

    public void SwapCursor() {
        if (dead) return;
        var mousePosition = Mouse.current.position.ReadValue();
        var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var distance = (Vector2)worldMousePosition - (Vector2)transform.position;
        float dir = distance.x >= 0 ? 1 : -1;
        if (distance.magnitude <= meleeAttackRange && mouseCursor == 1) {
            ChangeMouseCursor.instance.SetMeleeMouse();
            mouseCursor = 0;
        } else if(distance.magnitude > meleeAttackRange && mouseCursor == 0) {
            ChangeMouseCursor.instance.SetRangedMouse();
            mouseCursor = 1;
        }
    }

    public void PlayerMove(InputAction.CallbackContext context) {
        if (dead) return;
        if (isAttacking) return;
        if (isStoping) return;
        playerAxis = context.ReadValue<Vector2>();

        if (context.performed) {
            if (playerAxis.x != 0) {
                walkDustParticle.Play();
                lastInput = context;
                if (!canCountWalkValue) {
                    canCountWalkValue = true;
                }
            }
        }

        if (context.canceled) {
            if (walkValue >= resetWalkValue && IsGrounded()) {
                isStoping = true;
                playerAxis.x = 0;
                anim.SetTrigger("Stop");
                SpawnStopParticle();
                walkDustParticle.Stop();
                PlayerMove(context);
            }            
            canCountWalkValue = false;
            walkValue = 0;
        }
    }

    public void EnablePlayerMove() {
        isStoping = false;
        PlayerMove(lastInput);
    }

    public void Jump(InputAction.CallbackContext context) {
        if (dead) return;
        if (isAttacking) return;
        if (context.performed && IsGrounded()) {
            canCountWalkValue = false;
            jumping = true;
            anim.SetTrigger("Jump");
            playlist.PlaySFX("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.gravityScale = 4;
            SpawnParticles.instance.SpawnParticle(SpawnParticles.instance.newParticle("Dust"), dustPosition.position);
        } else if (context.canceled && rb.velocity.y > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * fallMultiply);
            rb.gravityScale = 2;
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (dead) return;
        if (context.performed) {

        }
    }

    public void Attack(InputAction.CallbackContext context) {
        if (dead) return;
        if (isAttacking || !IsGrounded()) return;
        if (currentWaterBallSize < minWaterBallSize + waterSpendValue) return;
        playerAxis.x = 0;
        if (context.performed) {
            if (isStoping) isStoping = false;
            walkValue = 0;
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
        walkDustParticle.Play();
        yield return new WaitForSeconds(attackSpeed);
        playerAxis.x = 0;
        yield return new WaitForSeconds(attackFireRate);
        walkDustParticle.Stop();
        isAttacking = false;
    }

    public void DisableBallFollow() {
        TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
        PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
        DOTween.To(() => ball.weight, (v) => ball.weight = v, 0, attackSpeed);
        trail.enabled = false;
        PlayerMove(lastInput);
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
        PlayerMove(lastInput);
    }

    public void SpecialAttack(InputAction.CallbackContext context) {
        if (dead) return;
        if (!IsGrounded()) return;
        if (isStoping) return;
        if (playerAxis.x != 0) return;

        if (context.performed && !isAttacking) {
            if (!charging) {
                if (isStoping) isStoping = false;
                charging = true;
                isAttacking = true;
                waterBall.GetComponent<WaterDropsGenerator>().StartSpawn();
                playlist.PlaySFX("Charging");
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
            rb.velocity = Vector2.zero;
            if (Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer)) {
                playerAxis.x = 0;
                if (isStoping) isStoping = false;
                PlayerMove(lastInput);
                jumping = false;
                canCountWalkValue = true;
                playlist.PlaySFX("landing");
                SpawnJumpParticle();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Enemy_Projectile") {
            TakeDamage();
        }
    }

    private void OnDrawGizmos() {
        if (showGizmos == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);
    }

    private bool IsGrounded() {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (!grounded) walkDustParticle.Stop();
        if (!grounded && lastInput.ReadValue<Vector2>().x == 0) playerAxis.x = 0;

        return grounded;
    }

    public void TakeDamage() {
        var damage = 1;
        if(currentPlayerHp - damage <= 0) {
            Death();
        } else {
            currentPlayerHp -= damage;
            SelectTakeDamageSound();
            anim.SetTrigger("Take Damage");
        }
        hpSlider.value = currentPlayerHp;
    }

    void Death() {
        dead = true;
        ChangeMouseCursor.instance.SetNormalMouse();
        currentPlayerHp = 0;
        playlist.PlaySFX("Death");
        anim.SetTrigger("Death");
        playerAxis.x = 0;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 4;
        walkDustParticle.Stop();
        GameConfig.instance.GameOver();
    }

    public void SelectTakeDamageSound() {
        var id = Random.Range(1,4);
        if (id > 3) id = 3;
        playlist.PlaySFX("Take Damage " + id);
    }

}