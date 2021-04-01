using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour {
    
    [SerializeField] int currentHealth;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask nyerts;
    [SerializeField] float speed;
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    [SerializeField] float viewFinder;
    [SerializeField] float forwardForce;
    
    private Animator animator;
    private GameObject blood;
    private CameraShake cameraShake;
    private float attackRange = 0.5f;
    private float cooldown;
    private float k_cooldown = 0.25f;
    private float jumpForce = 57.5f;
    private bool hit = true;
    private bool hurt;
    private bool movingLeft = true;
    private AudioSource audioSource;
    private AudioClip hitSound;
    private Rigidbody2D rb;
    private int maxHealth = 150;

    string[] dropNames = {"Zombie Spleen", "Zombie Spleen", "Zombie Spleen"};
    int[] dropRates = {12, 6, 3};

    void Start() {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hitSound = Resources.Load("hit") as AudioClip;
        blood = Resources.Load("Blood") as GameObject;
        currentHealth = maxHealth;
        cooldown = 0f;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        int random = Mathf.RoundToInt(Random.Range(0, 50));
        hurt = true;
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Update() {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();

        if (cooldown <= 0) {
            hit = true;
        } else {
            hit = false;
            cooldown -= Time.deltaTime;
        }

        if (movingLeft) {
            viewFinder = .2f;
            forwardForce = -2f;
        } else {
            viewFinder = -.2f;
            forwardForce = 2f;
        }
        
        if (hurt) {
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        }

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 1f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        #region String Builder
        string ground;
        string block;
        string air;

        bool NameCheck(RaycastHit2D hit2D, string objectName) {
            if (hit2D.collider && hit2D.collider.gameObject.name.Contains(objectName)) {
                return true;
            } else {
                return false;
            }
        }

        if (groundInfo.collider) {
            ground = groundInfo.collider.tag;
        } else {
            ground = "Nothing";
        }

        if (airInfo.collider && airInfo.collider.tag != "NT" && airInfo.collider.tag != "Water" && !NameCheck(airInfo, "Gunflower") && !NameCheck(airInfo, "Rock")) {
            air = airInfo.collider.tag;  
        } else {
            air = "Nothing";
        }

        if (blockInfo.collider && blockInfo.collider.tag != "NT" && blockInfo.collider.tag != "Water" && !NameCheck(blockInfo, "Gunflower") && !NameCheck(blockInfo, "Rock")) {
            block = blockInfo.collider.tag;
        } else {
            block = "Nothing";
        }
        #endregion

        if (ground == "Nothing" || (block == "Enemy" && blockInfo.collider.gameObject != gameObject) || air == "Ground") {
            FlipEnemy();
        }

        if ((block == "Ground" || block == "Bounce Pad" || block == "Block") && air == "Nothing") {
            Jump();
        }

        if ((block == "Player" || NameCheck(blockInfo, "Nyert")) && hit && !blockInfo.collider.GetComponent<PlayerMovement>().isDead) {
            Hit(10);
        }
    }

    void Jump() {
        rb.AddForce(new Vector2(forwardForce, jumpForce));
    }

    void FlipEnemy() {
        if (movingLeft == true) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = false;
        } else {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingLeft = true;
        }
    }

    public IEnumerator HitLogic(int damage) {
        audioSource.PlayOneShot(hitSound, 1f);
        animator.SetBool("Hit", true);
        StartCoroutine(cameraShake.Shake(.15f, .05f));
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, targetLayer);
        Collider2D[] hitNyerts = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, nyerts);
        
        foreach (Collider2D player in hitPlayer) {
            player.GetComponent<PlayerCombat>().TakeDamage(damage);
        }

        foreach (Collider2D nyert in hitNyerts) {
            nyert.GetComponent<NyertCombat>().TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Hit", false);
    }

    void Hit(int damage) {
        StartCoroutine(HitLogic(damage));
        hit = false;
        cooldown = k_cooldown;
    }

    public void Die() {
        StartCoroutine(Blood());
        GenerateDrops();
        HelperFunctions.IncrementInt("Zombies Killed");
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(blood, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    void GenerateDrops() {
        for (int i = 0; i < dropNames.Length; i++) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= dropRates[i]) {
                Drop(dropNames[i], 1);
            }
        }

        Drop("Clay Slab", 1); Drop("Wood Shard", 1);
    }

    void Drop(string name, int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject drop = Resources.Load("Physical Items/" + name) as GameObject;
            GameObject dropInstance = Instantiate(drop, transform.position, transform.rotation);
            dropInstance.name = string.Format("{0}x{1}", name, amount.ToString());
        }
    }

}
