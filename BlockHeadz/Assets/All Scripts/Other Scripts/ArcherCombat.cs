using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherCombat : MonoBehaviour {
    
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform sightPoint; 
    [SerializeField] GameObject blood;
    [SerializeField] Transform target;

    public int currentHealth;
    
    private Animator animator;
    private float hitDistance = 50f;
    private float distance;
    private Vector3 direction;
    private Vector3 sightDirection;
    private RaycastHit2D lookingAt;
    private int maxHealth = 200;
    private CameraShake cameraShake;
    
    # region Drops
    string[] dropNames = {"Eyeball", "Wood Shard", "Wood Shard", "Stone", "Clay Slab"};
    int[] dropRates = {50, 75, 100, 75, 75};
    # endregion

    private void Start() {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Update() {
        if (target == null) {
            target = GameObject.Find("Player").transform;
        }
        
        distance = Vector3.Distance(target.transform.position, transform.position);
        direction = firePoint.position - target.position;
        sightDirection = sightPoint.position - target.position;
        lookingAt = Physics2D.Linecast(sightPoint.position, target.position);
        Debug.DrawLine(sightPoint.position, target.position);

        #region String Builder
        string sight;

        if (lookingAt.collider) {
            sight = lookingAt.collider.tag;
        } else {
            sight = "Nothing";
        }
        #endregion

        if (sight != "Ground" && distance < hitDistance) {
            Fire();
        }

        FaceTarget();
    }

    void Fire() {
        animator.SetTrigger("Fire");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Archer_Fire")) {
            GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation) as GameObject;
            projectileInstance.GetComponent<Rigidbody2D>().AddForce(-direction * 250f); // power
            Destroy(projectileInstance, 2f);
        }
    }

    void FaceTarget() {
        if (target.position.x < transform.position.x) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        } else {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void TakeDamage(int damage) {
        animator.SetTrigger("Hurt");
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
        PlayerPrefsHelper.IncrementInt("Archers Killed"); 
        GenerateDrops();     
        Destroy(gameObject);
    }

    void GenerateDrops() {
        for (int i = 0; i < dropNames.Length; i++) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= dropRates[i]) {
                Drop(dropNames[i]);
            }
        }

        Drop("Clay Slab"); Drop("Wood Shard");
    }

    void Drop(string name) {
        GameObject drop = Resources.Load(name) as GameObject;
        Instantiate(drop, transform.position, transform.rotation);        
    }

}
