using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour {

    #region Variaveis

    public static Player instance;
    private Vector2 playerAxis;
    public Vector2 aim;
    public Vector2 aimAxis;
    public float playerVelocity;
    public float jumpForce;
    public float fallMultiply;
    private float walkValue;
    public float resetWalkValue;
    public float jumpingSlow;
    public float freio = 20;

    [Space(15)]
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
    private float currentWaterBallSize = 1.1f;
    public float waterBallIncreaseValue;
    public float waterSpendValue;

    [Space(15)]
    public int mouseCursor;
    public int maxPlayerHp;
    private int currentPlayerHp;
    public int playerDamage;

    [Space(15)]
    public bool facingRight;
    private bool isAttacking;
    private bool jumping;
    private bool countComboResetTime;
    private bool charging;
    private bool isStoping;
    private bool canCountWalkValue;
    public bool showGizmos;
    private bool dead;
    public bool cutscene;

    [Space(15)]
    public LayerMask groundLayer;
    public LayerMask interactLayer;
    public Transform groundCheck;
    public Transform dustPosition;
    public Transform stopPosition;
    private Rigidbody2D rb;
    private Animator anim;
    public ParticleSystem walkDustParticle;
    public Playlist playlist;
    public Slider waterSlider;
    public Slider hpSlider;

    [Space(15)]
    public GameObject waterBall;
    public GameObject waterProjectile;
    public GameObject hitbox;
    public SpriteRenderer meleeArea;

    private PlayerInput input;
    private InputAction.CallbackContext lastInput;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    public float meleeAttackAreaAlpha;
    public float meleeAttackAreaSpeedDelay;

    public TextMeshProUGUI objectiveText;
    public Save save;

    public bool controlAim;

    public float coyoteTime;
    public float currentCoyote;
    public bool coyote;

    public float inputCorrectionTime;
    public float currentinputCorrection;
    public bool inputCorrection;

    CallbackContext lastJump;


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
        Objetive();
    }

    void Update() {
        if (dead) return;
        if (cutscene) return;
        if (IsGrounded()) rb.velocity = new Vector2(playerAxis.x * playerVelocity, rb.velocity.y);
        else rb.velocity = new Vector2(playerAxis.x * (playerVelocity - jumpingSlow), rb.velocity.y);

        if (coyote) {
            if (rb.velocity.y < 0 && !jumping) {
                if (currentCoyote > coyoteTime) {
                    coyote = false;
                } else currentCoyote += Time.deltaTime;
            }
        }

        if (inputCorrection) {
            if (rb.velocity.y <= 0 ) {
                if (currentinputCorrection > inputCorrectionTime) {
                    inputCorrection = false;
                } else {
                    currentinputCorrection += Time.deltaTime;
                    if (IsGrounded()) Jump(lastJump);
                }
            }
        }

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

        if (controlAim) aim = aimAxis + (Vector2)transform.position;
        else aim = (Vector2)transform.position;

        if (playerAxis.x != 0 && canCountWalkValue) walkValue += Mathf.Abs(playerAxis.x) * Time.deltaTime;

        waterSlider.value = currentWaterBallSize;

        PassAnimationValues();
        meleeArea.gameObject.transform.localScale = Vector2.one * meleeAttackRange;

        TryResetCombo();
        TryFlip();
    }

    private void LateUpdate() {
        SwapCursor();
    }

    #region Controles

    public void EnablePlayerControls() {
        Debug.Log("Enable Player Actions");
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
        if (cutscene) return;
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
        if (cutscene) return;
        if (isAttacking) return;
        if (isStoping) return;
        playerAxis = context.ReadValue<Vector2>();

        if (context.performed) {
            if (isAttacking) isAttacking = false;
            if (playerAxis.x != 0) {
                walkDustParticle.Play();
                lastInput = context;
                if (!canCountWalkValue) {
                    canCountWalkValue = true;
                }
            }
        }

        if (context.canceled) {
            if (isAttacking) isAttacking = false;
            if (walkValue >= resetWalkValue && IsGrounded()) {
                
                isStoping = true;
                playerAxis.x = 0;
                anim.SetTrigger("Stop");
                SpawnStopParticle();
                walkDustParticle.Stop();
                rb.velocity = new Vector2(freio, rb.velocity.y);
                PlayerMove(context);
            }         
            canCountWalkValue = false;
            walkValue = 0;
        }
    }

    public void EnablePlayerMove() {
        isStoping = false;
        if (isAttacking) isAttacking = false;
        PlayerMove(lastInput);
    }

    public void Jump(InputAction.CallbackContext context) {
        if (dead) return;
        if (cutscene) return;
        if (isAttacking) return;

        if(context.performed && !IsGrounded() && !inputCorrection) {
            inputCorrection = true;
            lastJump = context;
        }

        if (context.performed && IsGrounded()) {
            currentCoyote = 0;
            currentinputCorrection = 0;
            coyote = false;
            inputCorrection = false;
            canCountWalkValue = false;
            jumping = true;
            anim.SetTrigger("Jumping");
            playlist.PlaySFX("jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.gravityScale = 4;
            SpawnParticles.instance.SpawnParticle("Dust", dustPosition.position);
        } else if (context.canceled && rb.velocity.y > 0f) {
            if (isAttacking) isAttacking = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * fallMultiply);
            rb.gravityScale = 4;
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (dead) return;
        if (cutscene) return;
        if (context.performed) {
            if (isAttacking) isAttacking = false;
            Collider2D[] obj = Physics2D.OverlapCircleAll(transform.position, meleeAttackRange, interactLayer);
            DOTween.ToAlpha(() => meleeArea.color, color => meleeArea.color = color, meleeAttackAreaAlpha, meleeAttackAreaSpeedDelay);
            foreach (var item in obj) {
                if (item.TryGetComponent<Interactable>(out Interactable interation)) {
                    interation.Interact();
                }
            }
            DOTween.ToAlpha(() => meleeArea.color, color => meleeArea.color = color, 0, meleeAttackAreaSpeedDelay);
        }
    }

    public void Aim(InputAction.CallbackContext context) {
        aimAxis = context.ReadValue<Vector2>();
        if (context.performed) controlAim = true;
        else if (context.canceled) controlAim = false;
    }

    public void Attack(InputAction.CallbackContext context) {
        if (dead) return;
        if (cutscene) return;
        if (isAttacking || !IsGrounded()) return;
        if (context.performed) DOTween.ToAlpha(() => meleeArea.color, color => meleeArea.color = color, meleeAttackAreaAlpha, meleeAttackAreaSpeedDelay);
        if (context.canceled) DOTween.ToAlpha(() => meleeArea.color, color => meleeArea.color = color, 0, meleeAttackAreaSpeedDelay);

        if (currentWaterBallSize <= minWaterBallSize) {
            VsnController.instance.StartVSN("no_water");
            return;
        }
        playerAxis.x = 0;
        if (context.performed) {
            if (isStoping) isStoping = false;
            
            walkValue = 0;

            var mousePosition = Vector2.zero;
            if (controlAim) {
                mousePosition = aim;
            } else {
                mousePosition = Mouse.current.position.ReadValue();
            }
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var distance = (Vector2)worldMousePosition - (Vector2)transform.position;
            float dir = distance.x >= 0 ? 1 : -1;
            if (distance.magnitude <= meleeAttackRange) MeleeAttack(dir);
            else RangedAttack();
        }        
    }

    private void MeleeAttack(float dir) {
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
        if (hitbox.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
            holder.damage = playerDamage * (int) (waterBall.transform.localScale.x * waterBallIncreaseValue);
            holder.doKnockback = true;
        }
        TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
        PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
        trail.enabled = true;
        DOTween.To(() => ball.weight, (v) => ball.weight = v, 1, attackSpeed);
        playerAxis.x = dir * stepDistance;
        walkDustParticle.Play();
        yield return new WaitForSeconds(stepStopTime);
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
        DOTween.ToAlpha(() => meleeArea.color, color => meleeArea.color = color, 0, meleeAttackAreaSpeedDelay);
    }

    public void RangedAttack() {
        isAttacking = true;
        anim.SetTrigger("RangedAttack");
    }

    public void UseRangedAttack() {
        SpendWater();
        GameObject newProjectile = Instantiate(waterProjectile, waterBall.transform.position, Quaternion.identity);
        newProjectile.transform.localScale = newProjectile.transform.localScale * currentWaterBallSize;

        if (newProjectile.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
            holder.damage = playerDamage;
            holder.doKnockback = false;
        }
        var mousePosition = Vector2.zero;
        if (controlAim) {
            mousePosition = aim;
        } else {
            mousePosition = Mouse.current.position.ReadValue();
        }
        var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        var distance = (Vector2)worldMousePosition - (Vector2)transform.position;
        var direction = (Vector2)worldMousePosition - (Vector2)waterBall.transform.position;

        newProjectile.GetComponent<Rigidbody2D>().velocity = (direction.normalized * projectileSpeed);
        newProjectile = null;

        DOTween.ToAlpha(() => meleeArea.color, color => meleeArea.color = color, 0, meleeAttackAreaSpeedDelay);
        HorizontalImpulse(distance);
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Possivel Resolução do Bug de Ficar Preso
        //if (isAttacking) isAttacking = false;
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
        if (isAttacking) isAttacking = false;
        PlayerMove(lastInput);
    }

    public void SpecialAttack(InputAction.CallbackContext context) {
        if (dead) return;
        if (cutscene) return;
        if (!IsGrounded()) return;
        if (isStoping) return;
        if (playerAxis.x != 0) return;

        if (context.performed && !isAttacking) {
            if (!charging) {
                if (isAttacking) isAttacking = false;
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
        if (currentWaterBallSize - waterSpendValue >= minWaterBallSize) {
            currentWaterBallSize -= waterSpendValue;
        }
        else if (currentWaterBallSize - waterSpendValue < minWaterBallSize) {
            currentWaterBallSize = minWaterBallSize;
        }
        waterBall.transform.localScale = Vector2.one * currentWaterBallSize;
    }

    public void SpawnStopParticle() {
        SpawnParticles.instance.SpawnParticle("Stop", stopPosition.position, transform.localScale.x);
    }

    public void SpawnJumpParticle() {
        SpawnParticles.instance.SpawnParticle("Dust2", dustPosition.position, transform.localScale.x);
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Ground") {
            rb.velocity = Vector2.zero;
            if (Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer)) {
                playerAxis.x = 0;
                currentCoyote = 0;
                coyote = false;
                if (isStoping) isStoping = false;
                if (isAttacking) isAttacking = false;
                PlayerMove(lastInput);
                jumping = false;
                canCountWalkValue = true;
                playlist.PlaySFX("landing");
                DisableAttack();
                SpawnJumpParticle();
            }
        }
        if(col.gameObject.tag == "Death") {
            TakeDamage(999);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (cutscene) return;
        if (col.tag == "Enemy_Projectile") {
            if (col.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
                if (holder.canDealDamage) TakeDamage(holder.damage);
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
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (!grounded) {
            walkDustParticle.Stop();
            if (!jumping) {
                if (rb.velocity.y < 0 && !coyote) {
                    coyote = true;
                }
            } else {
                if (coyote) coyote = false;
            }
        }
        if (!grounded && lastInput.ReadValue<Vector2>().x == 0) playerAxis.x = 0;
        return coyote == true ? coyote : grounded;
    }

    public void TakeDamage(int damage) {
        if (cutscene) return;
        if (currentPlayerHp - damage <= 0) {
            Death();
        } else {
            CameraShake.instance.Shake();
            currentPlayerHp -= damage;
            SelectTakeDamageSound();
            SpawnParticles.instance.SpawnParticle("Hit2", transform.position);
            anim.SetTrigger("Take Damage");
        }
        hpSlider.value = currentPlayerHp;
    }

    void Death() {
        dead = true;
        ChangeMouseCursor.instance.SetNormalMouse();
        currentPlayerHp = 0;
        SpawnParticles.instance.SpawnParticle("Blood", transform.position);
        playlist.PlaySFX("Death");
        anim.SetTrigger("Death");
        playerAxis.x = 0;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 4;
        if (isAttacking) isAttacking = false;
        walkDustParticle.Stop();
        GameConfig.instance.GameOver();
    }

    public void SelectTakeDamageSound() {
        var id = Random.Range(1,4);
        if (id > 3) id = 3;
        playlist.PlaySFX("Take Damage " + id);
    }

    public void ActiveMeleeDamate() {
        if (hitbox.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
            holder.damage = playerDamage;
            holder.canDealDamage = true;
        }
    }

    public void DisableMeleeDamate() {
        if (hitbox.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
            holder.canDealDamage = false;
        }
    }

    public void StopPlayer() {
        cutscene = true;
        if (isAttacking) isAttacking = false;
        DisableAttack();
        playerAxis.x = 0;
        rb.velocity = Vector2.zero;
        enabled = false;
    }

    public void BeginInitialCutscene() {
        cutscene = true;
        DisableAttack();
        playerAxis.x = 0;
        if(rb != null) rb.velocity = Vector2.zero;
        anim.SetTrigger("Begin");
    }

    public void FinishInitialCutscene() {
        cutscene = false;
        anim.Play("Ground");
        save.Reset();
    }

    public void ChangeObjetive(string nextObjective) {
        save.objective = nextObjective;
    }

    public void Objetive() {
        objectiveText.text = save.objective;
    }

    public void Cura(int cura) {
        if (currentPlayerHp + cura > maxPlayerHp) {
            currentPlayerHp = maxPlayerHp;
        } else {
            currentPlayerHp += cura;
        }
        hpSlider.value = currentPlayerHp;
    }

}