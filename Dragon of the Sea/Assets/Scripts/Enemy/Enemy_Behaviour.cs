using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    public enum enemy_type {Patrol, Wait, Invader }

    public enemy_type type = new();
    public bool showGizmo;
    private Rigidbody2D rb;
    private Animator anim;

    [Space(20)]
    [Header("Life")]
    public int maxLife;
    private int currentLife;

    [Space(20)]
    [Header("Combat")]
    public int damage;
    [Range(0, 20)]
    public float attackRadious;
    private float currentAttackTime;
    public float attackFireRate;
    public float shotSpeed;
    public GameObject projectile;
    public GameObject fireEffect;
    public Transform shotPosition;

    [Space(20)]
    [Header("Movement")]
    public float speed;
    [Range(0,25)]
    public float searchRadious;
    public float searchTime;
    private float currentSearchTime;
    public LayerMask playerLayer;

    [Space(20)]
    [Header("Extra")]
    public Playlist playlist;
    [SerializeField] private Transform player;
    private float playerDistance;
    private bool busy;
    private bool persuit;
    private bool invade;
    public Transform waitPoint;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playlist = GetComponent<Playlist>();
    }

    private void Update() {
        UpdateBehaviour();

        if(busy) {
            PersuitAndAttack();
        }
        anim.SetFloat("speed", rb.velocity.x);
    }

    void UpdateBehaviour() {
        if (player == null && !busy) {
            currentSearchTime += Time.deltaTime;
            if (type == enemy_type.Wait) {
                if (transform.position.x != waitPoint.transform.position.x) {
                    var homeDir = waitPoint.transform.position.x - transform.position.x;
                    var dir = homeDir >= 0 ? 1 : -1;
                    var homeSpeed = speed * dir;
                    transform.localScale = new Vector2(dir, 1);
                    rb.velocity = new Vector2(homeSpeed, 0);
                } else {
                    transform.position = new Vector2(waitPoint.transform.position.x, transform.position.y);
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector2(waitPoint.transform.position.x, transform.position.y);
                    rb.velocity = Vector2.zero;
                }
            }
            if (currentSearchTime >= searchTime) {
                currentSearchTime = 0;
                SearchPlayer();
            }
        } else if (player != null) {
            playerDistance = player.position.x - transform.position.x;
            if (MathF.Abs(playerDistance) > searchRadious) {
                player = null;
                currentAttackTime = 0;
                if (persuit) persuit = false;
                if (invade) invade = false;
                if (busy) busy = false;                
            }
        }
    }

    public void SearchPlayer() {
        if (player != null) return;
        Collider2D target = Physics2D.OverlapCircle(transform.position, searchRadious, playerLayer);
        if (target == null) return;
        player = target.transform;
        busy = true;
        DoAction();
    }

    void DoAction() {
        switch (type) {
            case enemy_type.Patrol:
                persuit = true;
                break;
            case enemy_type.Wait:
                persuit = true;
                break;
            case enemy_type.Invader:
                invade = true;
                break;
        }
    }

    void PersuitAndAttack() {
        if (persuit) {
            var dir = playerDistance >= 0 ? 1 : -1;            
            if (Mathf.Abs(playerDistance) <= attackRadious) {
                if(rb.velocity.x != 0) rb.velocity = Vector2.zero;
                currentAttackTime += Time.deltaTime;
                transform.localScale = new Vector2(dir, 1);
                if (currentAttackTime > attackFireRate) {
                    currentAttackTime = 0;
                    Attack(dir);
                }
            } else {
                rb.velocity = new Vector2(speed * dir, 0);
            }
        }
    }

    void Invade() {

    }

    void Attack(int dir) {
        anim.SetTrigger("Attack");
        GameObject bullet = Instantiate(projectile, shotPosition.position, Quaternion.identity);
        GameObject fire = Instantiate(fireEffect, shotPosition.position, Quaternion.identity);
        bullet.transform.localScale = new Vector2(dir, 1);
        fire.transform.localScale = new Vector2(dir, 1);
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shotSpeed * dir, 0);
    }

    private void OnDrawGizmos() {
        if (showGizmo) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, searchRadious);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadious);
        }
    }
}