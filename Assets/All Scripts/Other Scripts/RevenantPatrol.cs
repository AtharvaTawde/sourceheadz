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
    [SerializeField] GameObject onFireGraphic;

    public float burnTime = 0f;

    private int maxHealth = 1000;
    private float burnCooldown = 0f;
    private int currentWayPoint = 0;
    private float nextWayPointDistance = 1f; 
    private Vector3 direction;
    private RaycastHit2D lookingAt;
    private Path path;
    private Seeker seeker;
    private Rigidbody2D rb;
    private CameraShake cameraShake;
    private Animator animator;
    private GameObject touched;
    private GameObject healthEffect;
    private float hitCooldown;
    private bool hurt;

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"Gold Scrap1",     3},
        {"Phlegm Lattice1", 5},
        {"Gold Scrap2",     6},
        {"Phlegm Lattice2", 10},
        {"Gold Scrap3",     11},
        {"Gold Scrap4",     12},
        {"Gold Scrap5",     20},
        {"Corpuscle",       75},
    };

    private void Start() {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthEffect = Resources.Load("Plus Particles") as GameObject;
        hitCooldown = 0f;
        target = GameObject.Find("Player").transform;
        InvokeRepeating("UpdatePath", 0f, 0.25f);
    }
    
    private void Update() {
        CreatePath();
        Burn();

        if (hurt)
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;

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

        if (sight != "Ground" && sight != "Block" && !target.GetComponent<PlayerMovement>().isDead) {
            if (hitCooldown <= 0) {
                Fire();
                hitCooldown = Random.Range(3, 5);
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

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        
        if (distanceAwayFromTarget.IsWithin(0, 25)) {
            if (!target.GetComponent<PlayerMovement>().isDead) {
                rb.AddForce(direction * speed);
            } else {
                rb.AddForce(-direction * speed);
            }
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

    #endregion

    #region Combat Stuff
    public void TakeDamage(int damage) {
        hurt = true;
        currentHealth -= damage;
        
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        HelperFunctions.IncrementInt("Revenants Killed");
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
        foreach (KeyValuePair<string, int> entry in drops) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= entry.Value) {
                Drop(entry.Key.RemoveIntegers(), 1);
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

    #region Attack Form 1 (Contact)
    private void OnTriggerEnter2D(Collider2D other) {
        touched = other.gameObject;
        int touchDamage = Random.Range(5, 7);

        if (touched.tag == "Player") {
            if (!touched.GetComponent<PlayerMovement>().isDead) {
                touched.GetComponent<PlayerCombat>().TakeDamage(touchDamage);
            }
        } else if (touched.tag == "Enemy") {
            string[] enemyNames = {"Nyert", "Goblin", "Zombi", "Archer", "Fallen"};
            foreach (string n in enemyNames) {
                if (Name(n)) {
                    touched.GetComponent<TakeDamage>().ReceiveDamage(touchDamage);
                } 
            }
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
