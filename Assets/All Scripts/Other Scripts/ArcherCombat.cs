using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherCombat : MonoBehaviour {
    
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform sightPoint; 
    [SerializeField] GameObject blood;
    [SerializeField] Transform target;
    [SerializeField] GameObject onFireGraphic;

    public int currentHealth;
    public float burnTime = 0f;
    
    private Animator animator;
    private Rigidbody2D rb;
    private float hitDistance = 50f;
    private float distance;
    private float burnCooldown = 0f;
    private Vector3 direction;
    private Vector3 sightDirection;
    private RaycastHit2D lookingAt;
    private int maxHealth = 400;
    private CameraShake cameraShake;
    private bool hurt;
    
    # region Drops
    string[] dropNames = {"Eyeball", "Wood Shard", "Wood Shard", "Stone", "Clay Slab"};
    int[] dropRates = {50, 75, 100, 75, 75};
    # endregion

    private void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Update() {
        if (target == null) {
            target = GameObject.Find("Player").transform;
        }

        Burn();

        if (hurt)
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        
        Vector3 actualTarget = new Vector3(target.transform.position.x, target.transform.position.y - 0.5f, 0f);
        distance = Vector3.Distance(actualTarget, transform.position);
        direction = firePoint.position - actualTarget;
        sightDirection = sightPoint.position - actualTarget;
        lookingAt = Physics2D.Linecast(sightPoint.position, actualTarget);
        Debug.DrawLine(sightPoint.position, actualTarget);

        #region String Builder
        string sight;

        if (lookingAt.collider) {
            sight = lookingAt.collider.tag;
        } else {
            sight = "Nothing";
        }
        #endregion

        if ((sight != "Ground" && sight != "Block") && distance < hitDistance && (!target.GetComponent<PlayerMovement>().isDead || target.GetComponent<PlayerMovement>() == null)) {
            Fire();
        }

        FaceTarget();
    }

    void Fire() {
        animator.SetTrigger("Fire");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Archer_Fire")) {
            GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation) as GameObject;
            projectileInstance.GetComponent<Rigidbody2D>().AddForce(-direction * 250f); // power
            Destroy(projectileInstance, 15f);
        }
    }

    void FaceTarget() {
        if (target.position.x < transform.position.x) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    void Burn() {
        if (burnTime > 0) {
            onFireGraphic.SetActive(true);
            burnTime -= Time.deltaTime;

            if (burnCooldown <= 0) {
                TakeDamage(10);                
                burnCooldown = 0.5f;
            } else {
                burnCooldown -= Time.deltaTime;
            }
        } else {
            onFireGraphic.SetActive(false);
            burnTime = 0;
        }
    }

    public void TakeDamage(int damage) {
        hurt = true;
        currentHealth -= damage;

        if (currentHealth <= 0) {
            StartCoroutine(Blood());
            Die();
        }
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(blood, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    void Die() {
        StartCoroutine(Blood());
        HelperFunctions.IncrementInt("Archers Killed"); 
        GenerateDrops();     
        Destroy(gameObject);
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
