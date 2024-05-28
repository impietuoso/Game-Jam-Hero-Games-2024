using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour {
    public enum enemy_type {Patrol, Wait, Invader }

    public enemy_type type = new();
    public bool showGizmo;
    private Rigidbody2D rb;
    private Animator anim;

    [Space(15)]
    [Header("Life")]
    public int maxLife;
    private int currentLife;

    [Space(15)]
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

    [Space(15)]
    [Header("Movement")]
    public float speed;
    [Range(0,25)]
    public float searchRadious;
    public float searchTime;
    private float currentSearchTime;
    public LayerMask playerLayer;
    public float impulseForce;

    [Space(15)]
    [Header("Extra")]
    public Playlist playlist;
    [SerializeField] private Transform player;
    private float playerDistance;
    private bool busy;
    private bool patrol;
    private bool persuit;
    private bool invade;
    private bool dead;
    public Transform waitPoint;
    private Transform currentPatrolPoint;
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public Slider hpSlider;

    public bool stunned;
    public float stunTime;

    public Transform groundCheck;
    public LayerMask groundLayer;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playlist = GetComponent<Playlist>();
        hpSlider.maxValue = maxLife;
        currentLife = maxLife;
        hpSlider.value = currentLife;
        currentPatrolPoint = patrolPoint2;
    }

    private void Update() {
        if (dead) return;
        if (stunned) return;
        if (IsGrounded() == false) return;
        if (type == enemy_type.Invader) InvaderBehaviour();
        else UpdateBehaviour();

        if (busy) {
            PersuitAndAttack();
        }
        anim.SetFloat("speed",MathF.Abs(rb.velocity.x));
    }

    void UpdateBehaviour() {
        if (player == null && !busy) {
            currentSearchTime += Time.deltaTime;
            if (type == enemy_type.Wait) {
                var homeDir = waitPoint.transform.position.x - transform.position.x;

                if (Mathf.Abs(homeDir) > 0.5f) {
                    var dir = homeDir >= 0 ? 1 : -1;
                    var homeSpeed = speed * dir;
                    transform.localScale = new Vector2(dir, 1);
                    rb.velocity = new Vector2(homeSpeed, 0);
                } else {
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector2(waitPoint.transform.position.x, transform.position.y);                    
                }
            } else if(type == enemy_type.Patrol) {
                var homeDir = currentPatrolPoint.transform.position.x - transform.position.x;

                if (Mathf.Abs(homeDir) > 0.2f) {
                    var dir = homeDir >= 0 ? 1 : -1;
                    var homeSpeed = speed/2 * dir;
                    transform.localScale = new Vector2(dir, 1);
                    rb.velocity = new Vector2(homeSpeed, 0);
                } else {
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector2(currentPatrolPoint.transform.position.x, transform.position.y);
                    if (currentPatrolPoint == patrolPoint1) currentPatrolPoint = patrolPoint2;
                    else currentPatrolPoint = patrolPoint1;
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

    void InvaderBehaviour() {
        if (player == null) player = Player.instance.transform;
        playerDistance = player.position.x - transform.position.x;
        var dir = playerDistance >= 0 ? 1 : -1;
        if (Mathf.Abs(playerDistance) <= attackRadious) {
            if (rb.velocity.x != 0) rb.velocity = Vector2.zero;
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

    public void SearchPlayer() {
        if (player != null) return;
        Collider2D target = Physics2D.OverlapCircle(transform.position, searchRadious, playerLayer);
        if (target == null) return;
        player = target.transform;
        playlist.PlaySFX("Alert");
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
        playlist.PlaySFX("Attack");
        GameObject bullet = Instantiate(projectile, shotPosition.position, Quaternion.identity);
        GameObject fire = Instantiate(fireEffect, shotPosition.position, Quaternion.identity);
        bullet.transform.localScale = new Vector2(dir, 1);
        fire.transform.localScale = new Vector2(dir, 1);
        if (bullet.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
            holder.damage = damage;
        }
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(shotSpeed * dir, 0);
    }

    private void OnDrawGizmos() {
        if (showGizmo) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, searchRadious);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRadious);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(groundCheck.position, 0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (dead) return;
        if (stunned) return;
        if (col.tag == "PlayerHitBox") {
            if (col.TryGetComponent<DamageHolder>(out DamageHolder holder)) {
                if(holder.canDealDamage) TakeDamage(holder.damage);
            }
        }
    }

    public void TakeDamage(int damage) {
        if (dead) return;
        currentAttackTime = 0;
        if (currentLife - damage <= 0) {
            Death();
        } else {
            SpawnParticles.instance.SpawnParticle("Hit1", transform.position);
            Impulse();
            currentLife -= damage;
            SelectTakeDamageSound();
            anim.SetTrigger("Take Damage");
            StartCoroutine(Stun());
        }
        hpSlider.value = currentLife;
    }

    IEnumerator Stun() {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }

    private void Impulse() {
        if (dead) return;
        rb.AddForce(new Vector2(0, 100), ForceMode2D.Force);
        var distance = transform.position.x - Player.instance.transform.position.x;
        var direction = distance >= 0 ? 1 : -1;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * speed * impulseForce, 100), ForceMode2D.Force);
    }

    void Death() {
        SpawnParticles.instance.SpawnParticle("Blood", transform.position);
        dead = true;
        rb.velocity = Vector2.zero;
        currentLife = 0;
        playlist.PlaySFX("Death");
        anim.SetTrigger("Death");
    }

    void SelectTakeDamageSound() {
        var id = UnityEngine.Random.Range(1, 5);
        if (id > 4) id = 4;
        playlist.PlaySFX("Take Damage " + id);
    }

    private bool IsGrounded() {

        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer); ;
    }

    public void RemoveEnemy() {
        Destroy(gameObject.transform.parent.gameObject, 2.5f);
    }
}