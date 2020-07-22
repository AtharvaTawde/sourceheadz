using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public Transform attackPoint;
    public Transform bottleDetection;
    public Transform keyDetection; 
    public Transform groundDetection;
    public Transform gateDetection;
    public float attackRange = 0.5f;
    public int attackDamage = 20;

    public LayerMask enemyLayers;
    public LayerMask goblins;
    public LayerMask healthpotions; 
    public LayerMask keys;
    public LayerMask limit;
    public LayerMask gateway;

    public int maxHealth = 200;
    public int currentHealth;
    private bool isGrounded;
    private bool takesFallDamage;
    private int jumpHeight = 0;
    private int minimumHeight = 200;
    //public CameraShake shake;
    public Transform Explosion;
    public int HPOTS = 0;
    public bool hasKey = false;
    public bool opened = false;
    public GameObject Gateway;
    public bool thruGate = false;

    void Start() {
        currentHealth = maxHealth;
    }

    void Update() {
        isGrounded = GetComponent<CharacterController2D>().m_Grounded;
        
        FallDamage();
        DrinkHP();
        IsTouchingLimit();
        IsTouchingGate();

        if (Input.GetKeyDown(KeyCode.Space)) {
            Attack();     
        }

        if (currentHealth <= 0) {
            currentHealth = 0;
            maxHealth = 0;
        }

        if (hasKey) {
            Gateway.SetActive(true);
            opened = true;  
            hasKey = false;
        }
    }

    public void DrinkHP() {
        if (Input.GetKeyDown(KeyCode.E) && HPOTS > 0) {
            HPOTS -= 1;
            currentHealth += 25;
        }

        if (currentHealth >= maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            PlayerDie();
        }
    }

    public void PlayerDie() {
        currentHealth = 0;
        if (gameObject.tag == "Player") {
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    public void FallDamage() {
        if (!isGrounded) {
            jumpHeight++;
            Debug.Log(jumpHeight);
        } else {
            jumpHeight = 0;
        }

        if (jumpHeight >= minimumHeight) {
            takesFallDamage = true;
        }

        if (takesFallDamage && isGrounded) {
            currentHealth -= -1 * (jumpHeight - minimumHeight) / 10;
            takesFallDamage = false;
            //StartCoroutine(shake.Shake(.15f, .4f));
        }

        if (currentHealth <= 0) {
            PlayerDie();
        }
    }

    public void Attack() {
        // Play animation
        animator.SetTrigger("Attack");
        // Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitGoblins = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, goblins);
        Collider2D[] hitHPots = Physics2D.OverlapCircleAll(bottleDetection.position, attackRange, healthpotions);
        Collider2D[] hitKeys = Physics2D.OverlapCircleAll(keyDetection.position, attackRange, keys);
        // Deal damage
        foreach(Collider2D enemy in hitEnemies) {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }

        foreach(Collider2D goblin in hitGoblins) {
            goblin.GetComponent<GoblinEnemy>().TakeDamage(attackDamage);
            goblin.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }

        foreach(Collider2D healthpotion in hitHPots) {
            healthpotion.GetComponent<Collection>().SelfDestruct();
            HPOTS += 1;
        }   

        foreach(Collider2D key in hitKeys) {
            key.GetComponent<Collection>().DestroyKey();
            hasKey = true; 
        }
    }

    public void IsTouchingLimit() {
        Collider2D[] hitBox = Physics2D.OverlapCircleAll(groundDetection.position, attackRange, limit);

        foreach(Collider2D box in hitBox) {
            PlayerDie();
        }
    }

    public void IsTouchingGate() {
        Collider2D[] hitGate = Physics2D.OverlapCircleAll(gateDetection.position, attackRange, gateway);

        foreach(Collider2D gate in hitGate) {
            opened = false;
            thruGate = true;
        }
    }
}
