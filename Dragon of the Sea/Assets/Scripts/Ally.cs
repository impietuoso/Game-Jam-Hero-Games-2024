using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ally : MonoBehaviour {

    public enum ally_mode { Locked, Freed }

    public ally_mode mode = new();

    public Rigidbody2D rb;
    public Animator anim;
    public Transform playerTarget;
    public float speed;
    public bool follow;
    public LayerMask groundLayer;
    public Transform groundCheck;
    bool facingRight;
    public int fadeDirection;
    public float fadeTime;
    int currentHappiness;
    int maxHappy = 3;

    private void Awake() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        playerTarget = Player.instance.transform;
        if(mode == ally_mode.Locked) Lock();
    }

    void Update() {
        if (mode == ally_mode.Freed) {
            if (follow && playerTarget != null) {
                if (IsGrounded()) FollowPlayer();

                if (rb.velocity.x > 0 && !facingRight) {
                    Flip();
                } else if (rb.velocity.x < 0 && facingRight) {
                    Flip();
                }
            }
        }        
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    public void Lock() {
        rb.gravityScale = 1;
        anim.SetTrigger("Lock");
    }

    public void Release() {
        rb.gravityScale = 0;
        anim.SetTrigger("Release");
        Player.instance.save.Rescue();
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer); ;
    }

    public void StartFollow() {
        currentHappiness++;
        if(currentHappiness >= maxHappy) {
            if (mode == ally_mode.Locked) {
                FadeNPC();
            } else
            if (!follow) follow = true;
        }        
    }

    void FollowPlayer() {
        var homeDir = playerTarget.transform.position.x - transform.position.x;
        if (Mathf.Abs(homeDir) > 0.5f) {
            var dir = homeDir >= 0 ? 1 : -1;
            var homeSpeed = speed * dir;
            transform.localScale = new Vector2(dir, 1);
            rb.velocity = new Vector2(homeSpeed, 0);
        } else {
            rb.velocity = Vector2.zero;
            //transform.position = new Vector2(playerTarget.transform.position.x, transform.position.y);
        }
    }

    public void FadeNPC() {
        rb.velocity = new Vector2(speed * fadeDirection, 0);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        DOTween.ToAlpha(() => sr.color, color => sr.color = color, 0, fadeTime);
    }

    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

    }

    private void OnDrawGizmos() {
        if (groundCheck == false) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
    }
}