using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firetrap : MonoBehaviour {
    
    [SerializeField] Transform target;
    [SerializeField] Transform firePoint; 
    [SerializeField] GameObject projectile;
    [SerializeField] int currentHealth;
    [SerializeField] float cooldown;
    [SerializeField] float contactCooldown; 
    [SerializeField] bool facingRight;
    [SerializeField] bool isTargetInFront;
    [SerializeField] PlayerMovement player;

    private float k_hitDistance = 10f;
    private float k_cooldown = 1.5f;
    private float k_contactCooldown = 0.25f;
    private float distance;
    private int maxHealth = 550;
    private Vector3 direction;
    private Vector3 fireBallSpawnVector;
    private RaycastHit2D lookingAt;
    private bool hurt;
    private bool fire;
    private Animator animator;
    private bool doDamage;
    private GameObject dealDamageTo;

    #region Drops
    string[] dropNames = {"Fireball", "Fireball", "Fireball", "Fireball", "Fireball", "Fireball", "Fireball", "Fireball"};
    int[] dropRates = {100, 50, 25, 12, 6, 3, 2, 1};
    #endregion

    private void Start() {
        currentHealth = maxHealth; 
        cooldown = 0f;
        contactCooldown = 0f;
        animator = GetComponent<Animator>();

        if (firePoint.position.x > transform.position.x) {
            facingRight = true;
            fireBallSpawnVector = new Vector3(0, 0, 90);
        } else {
            facingRight = false;
            fireBallSpawnVector = new Vector3(0, 0, -90);
        }
    }

    private void Update() {
        if (target == null) {
            target = GameObject.Find("Player").transform;
        }

        if (player == null) {
            player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        }

        if (cooldown <= 0) {
            fire = true;
        } else {
            cooldown -= Time.deltaTime;
        }

        if (doDamage && contactCooldown <= 0) {
            DealContactDamage(dealDamageTo);
        } else if (doDamage && contactCooldown > 0) {
            contactCooldown -= Time.deltaTime;
        }

        if (facingRight) {
            if (target.position.x > firePoint.position.x) {
                isTargetInFront = true;
            } else {
                isTargetInFront = false;
            }
        } else {
            if (target.position.x < firePoint.position.x) {
                isTargetInFront = true;
            } else {
                isTargetInFront = false;
            }
        }

        if (hurt) {
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        }

        Vector3 actualTarget = new Vector3(target.transform.position.x, target.transform.position.y - 0.5f, 0f);
        distance = Vector3.Distance(actualTarget, transform.position);
        direction = firePoint.position - actualTarget;
        lookingAt = Physics2D.Linecast(firePoint.position, actualTarget);
        Debug.DrawLine(firePoint.position, actualTarget);


        #region String Builder
        string sight;
        
        if (lookingAt.collider) {
            sight = lookingAt.collider.tag;
        } else {
            sight = "Nothing";
        }
        #endregion

        if (sight != "Ground" && sight != "Block" && distance < k_hitDistance && isTargetInFront && !player.isDead) {
            if (fire) {
                Fire();
            }
        } else {    
            animator.SetBool("Fire", false);
        }
    }

    void Fire() {
        animator.SetBool("Fire", true);
        GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation);
        projectileInstance.transform.eulerAngles = fireBallSpawnVector;
        projectileInstance.GetComponent<Rigidbody2D>().AddForce(-direction * 250f); // power
        Destroy(projectileInstance, 2f);
        fire = false;
        cooldown = k_cooldown;
    }

    public void TakeDamage(int damage) {
        hurt = true;
        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        HelperFunctions.IncrementInt("Firetraps Killed"); 
        GenerateDrops();
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        doDamage = true;
        dealDamageTo = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other) {
        doDamage = false;
        dealDamageTo = null;
    }

    void DealContactDamage(GameObject entity) {
        if (entity.GetComponent<TakeDamage>() != null) {
            entity.GetComponent<TakeDamage>().ReceiveDamage(10);
        } else if (entity.gameObject.tag == "Player") {
            entity.GetComponent<PlayerCombat>().TakeDamage(10);
        }
        contactCooldown = k_contactCooldown;
    }

    void GenerateDrops() {
        for (int i = 0; i < dropNames.Length; i++) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= dropRates[i]) {
                Drop(dropNames[i], 1);
            }
        }
    }

    void Drop(string name, int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject drop = Resources.Load("Physical Items/" + name) as GameObject;
            GameObject dropInstance = Instantiate(drop, transform.position, transform.rotation);
            dropInstance.name = string.Format("{0}x{1}", name, amount.ToString());
        }
    }
 
}
 