using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RevenantPatrol : MonoBehaviour {
    
    [SerializeField] int currentHealth;
    [SerializeField] GameObject blood;
    [SerializeField] Transform target;
    [SerializeField] Transform firePoint; 
    [SerializeField] float speed = 200f;
    [SerializeField] float distanceAwayFromTarget;
    [SerializeField] float anotherDistance;
    [SerializeField] GameObject projectile;

    private int maxHealth = 1000;
    private int currentWayPoint = 0;
    private float hitDistance = 10f;
    private float nextWayPointDistance = 1f; 
    private Vector3 direction;
    private RaycastHit2D lookingAt;
    private Path path;
    private Seeker seeker;
    private bool reachedEndOfPath = false;
    private Rigidbody2D rb;
    private CameraShake cameraShake;
    private Animator animator;
    private GameObject touched;
    private GameObject healthEffect;
    private static readonly float cooldown = .5f;
    private float hitCooldown;

    #region Drops
    string[] dropNames = {"Diamond", "Diamond", "Diamond", "Diamond", "Diamond", "Diamond", "Diamond", "Diamond", "Diamond", "Diamond", "Corpuscle", "Corpuscle", "Clay Slab", "Clay Slab"};
    int[] dropRates = {75, 50, 30, 20, 5, 5, 4, 3, 2, 1, 100, 100, 100, 100};
    #endregion 

    private void Start() {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthEffect = Resources.Load("Plus Particles") as GameObject;
        hitCooldown = 0f;

        InvokeRepeating("UpdatePath", 0f, 0.25f);
    }
    
    private void Update() {
        CreatePath();

        anotherDistance = Vector3.Distance(target.position, transform.position);
        direction = firePoint.position - target.position;
        lookingAt = Physics2D.Linecast(firePoint.position, target.position);
        Debug.DrawLine(firePoint.position, target.position);

        #region String Builder
        string sight;

        if (lookingAt.collider) {
            sight = lookingAt.collider.tag;
        } else {
            sight = "Nothing";
        }            
        #endregion

        if (sight != "Ground" && InBetween(15, 25)) {
            if (hitCooldown <= 0) {
                Fire();
                hitCooldown = cooldown;
            } else if (hitCooldown > 0) {
                hitCooldown -= Time.deltaTime;
            }
        }
    }

    #region Pathfinding
    private void CreatePath() {
        distanceAwayFromTarget = Vector3.Distance(target.position, transform.position);

        if (path == null) {
            return;
        }    

        if (currentWayPoint >= path.vectorPath.Count) {
            reachedEndOfPath = true;
            return;
        } else {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        
        if (InBetween(0, 25)) {
            rb.AddForce(direction * speed);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance) {
            currentWayPoint++;
        }

        if (rb.velocity.x >= .01f) {
            transform.localScale = new Vector3(-3, 3, 3);
        } else if (rb.velocity.x <= .01f) {
            transform.localScale = new Vector3(3, 3, 3);
        }
    }

    private void UpdatePath() {
        if (seeker.IsDone())    
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWayPoint = 0;
        }
    }

    private bool InBetween(int min, int max) {
        if (distanceAwayFromTarget <= max && distanceAwayFromTarget >= min) {
            return true;
        } else {
            return false;
        }
    }

    #endregion

    #region Combat Stuff
    public void TakeDamage(int damage) {
        animator.SetTrigger("Hurt");
        currentHealth -= damage;
        
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        PlayerPrefsHelper.IncrementInt("Revenants Killed");
        StartCoroutine(Blood()); 
        GenerateDrops();
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
    }

    void Drop(string name, int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject drop = Resources.Load(name) as GameObject;
            Instantiate(drop, transform.position, transform.rotation);
        }
    }

    #region Attack Form 1 (Contact)
    private void OnTriggerEnter2D(Collider2D other) {
        touched = other.gameObject;
        int touchDamage = Random.Range(5, 7);

        if (touched.tag == "Player") {
            touched.GetComponent<PlayerCombat>().TakeDamage(touchDamage);
        } else if (touched.tag == "Enemy") {
            if (Name("Nyert")) {
                touched.GetComponent<Enemy>().TakeDamage(touchDamage);
            } else if (Name("Goblin") || Name("Zombi")) {
                touched.GetComponent<GoblinEnemy>().TakeDamage(touchDamage);
            } else if (Name("Archer")) {
                touched.GetComponent<ArcherCombat>().TakeDamage(touchDamage);
            } else if (Name("Fallen")) {
                touched.GetComponent<FallenCombat>().TakeDamage(touchDamage);
            } 
        }
    }

    bool Name(string word) {
        if (touched.name.Contains(word) && touched != null) {
            return true;
        } else {
            return false;
        }
    }
    #endregion
    
    #region Attack Form 2 (Projectile)
    
    void Fire() {
        GameObject projectileInstance = Instantiate(projectile, firePoint.position, firePoint.rotation) as GameObject;
        projectileInstance.GetComponent<Rigidbody2D>().AddForce(-direction * 50f); // power
        Destroy(projectileInstance, 2f);
    }

    #endregion
    
    #endregion
}
