using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

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

    public bool facingRight;
    public bool showGizmos;
    public bool isAttacking;
    public bool jumping;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Rigidbody2D rb;
    public Animator anim;

    public GameObject waterBall;
    public GameObject waterProjectile;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start() {

    }

    void Update() {
        if(!isAttacking) rb.velocity = new Vector2(playerAxis.x * playerVelocity, rb.velocity.y);
        else if(isAttacking == true && rb.velocity.x != 0) {
            rb.velocity = Vector2.zero;
        }

        PassAnimationValues();

        TryFlip();
    }

    private void PassAnimationValues() {
        anim.SetFloat("HorizontalSpeed", Mathf.Abs(rb.velocity.x));
        int verticalSpeed = rb.velocity.y >= 0 ? 1 : 0;
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
        playerAxis = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context) {
        if (isAttacking) return;
        if (context.performed && IsGrounded()) {
            jumping = true;
            anim.SetTrigger("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        } else if (context.canceled && rb.velocity.y > 0f) {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * fallMultiply);
        }
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.performed) {

        }
    }

    public void Attack(InputAction.CallbackContext context) {
        if (isAttacking  || !IsGrounded()) return;
        if (context.performed) {
            var mousePosition = Mouse.current.position.ReadValue();
            var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var distance = (Vector2)worldMousePosition - (Vector2)transform.position;
            var projectileDir = (Vector2)worldMousePosition - (Vector2)waterBall.transform.position;
            if (distance.magnitude <= meleeAttackRange) MeleeAttack();
            else RangedAttack(projectileDir);
        }
    }

    private void MeleeAttack() {
        Debug.Log("Melee Attack");
        isAttacking = true;
        StartCoroutine(MeleeWaterballAttack());
    }

    private IEnumerator MeleeWaterballAttack() {
        TrailRenderer trail = waterBall.GetComponent<TrailRenderer>();
        PositionConstraint ball = waterBall.GetComponent<PositionConstraint>();
        //trail.enabled = true;
        DOTween.To(() => ball.weight, (v) => ball.weight = v, 1, attackSpeed);
        yield return new WaitForSeconds(attackSpeed);
        DOTween.To(() => ball.weight, (v) => ball.weight = v, 0, attackSpeed);
        //trail.enabled = false;
        yield return new WaitForSeconds(attackFireRate);
        isAttacking = false;
    }

    public void RangedAttack(Vector2 mousePos) {
        Debug.Log("Ranged Attack");
        isAttacking = true;
        anim.SetTrigger("RangedAttack");
    }

    public void UseRangedAttack(int finished) {
        GameObject newProjectile = Instantiate(waterProjectile, waterBall.transform.position, Quaternion.identity);
        var mousePosition = Mouse.current.position.ReadValue();
        var worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var dir = (Vector2)worldMousePosition - (Vector2)waterBall.transform.position;
        newProjectile.GetComponent<Rigidbody2D>().velocity = (dir * projectileSpeed);
        newProjectile = null;
        if (finished == 1) isAttacking = false;
    }

    public void SpecialAttack(InputAction.CallbackContext context) {
        if (context.performed) {

        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Ground") {
            if(Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer)) {
                jumping = false;
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