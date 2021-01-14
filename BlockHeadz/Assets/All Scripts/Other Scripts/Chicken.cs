using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Chicken : MonoBehaviour {
    
    [SerializeField] float speed;
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform backDetection;
    [SerializeField] Transform airDetection;
    [SerializeField] float viewFinder;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject onFireGraphic;
    [SerializeField] GameObject egg;
    [SerializeField] float timer;
    [SerializeField] RuntimeAnimatorController chickenController;
    [SerializeField] Sprite adultChicken;

    private Rigidbody2D rb;
    private bool movingLeft = true;
    private GameObject Explosion;    
    private bool hurt;
    private float burnCooldown = 0f;
    
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public float burnTime = 0f;
    private int leastAmountOfTimeToLayEgg = 90;
    private int mostAmountOfTimeToLayEgg = 120;

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"Chicken", 100}
    };

    private Dictionary<string, int> burnDrops = new Dictionary<string, int> {
        {"Cooked Chicken", 100}
    };

    private Dictionary<string, int> babyDrops = new Dictionary<string, int> {
        {"Chicken Nugget1", 100},
        {"Chicken Nugget2", 50},
        {"Chicken Nugget3", 25},
        {"Chicken Nugget4", 12},
        {"Chicken Nugget5", 6}
    };

    private Dictionary<string, int> babyBurnDrops = new Dictionary<string, int> {
        {"Cooked Chicken Nugget1", 100},
        {"Cooked Chicken Nugget2", 50},
        {"Cooked Chicken Nugget3", 25},
        {"Cooked Chicken Nugget4", 12},
        {"Cooked Chicken Nugget5", 6}
    };
  
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = transform.GetChild(0);
        blockDetection = transform.GetChild(1);
        airDetection = transform.GetChild(2);
        backDetection = transform.GetChild(3);
        jumpForce = Mathf.Sqrt(Physics2D.gravity.magnitude * rb.gravityScale / 16) * rb.mass;
        currentHealth = maxHealth;
        Explosion = Resources.Load("Blood") as GameObject;
        timer = Random.Range(leastAmountOfTimeToLayEgg, mostAmountOfTimeToLayEgg);
    }

    private void OnValidate() { 
        groundDetection = transform.GetChild(0);
        blockDetection = transform.GetChild(1);
        airDetection = transform.GetChild(2);
        backDetection = transform.GetChild(3);
    }
     
    void Update() {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (movingLeft) {
            viewFinder = .2f;
        } else {
            viewFinder = -.2f;
        }

        // Speed Adjustment Based on Variation
        if (name.Contains("Baby") && speed != 4) {
            speed = 4;
        } else if (!name.Contains("Baby") && speed != 1) {
            speed = 1;
        }

        // If Baby, Age. If Adult, Lay Egg.
        if (name.Contains("Baby")) {
            if (timer > 0) {
                timer -= Time.deltaTime;
            } else {
                timer = 0;
                name = "Chicken";
                GetComponent<SpriteRenderer>().sprite = adultChicken;
                animator.runtimeAnimatorController = chickenController;
                timer = Random.Range(leastAmountOfTimeToLayEgg, mostAmountOfTimeToLayEgg);
            }
        } else {
            if (timer > 0) {
                timer -= Time.deltaTime;
            } else {
                timer = 0;
                int random = Random.Range(0, 100);

                if (random <= 50) {
                    Instantiate(egg, new Vector3(transform.position.x, transform.position.y - 0.3f, 0), transform.rotation);
                    leastAmountOfTimeToLayEgg += 5;
                    mostAmountOfTimeToLayEgg += 5;
                } else {
                    leastAmountOfTimeToLayEgg -= 20;
                    mostAmountOfTimeToLayEgg -= 20;
                }
                
                timer = Random.Range(leastAmountOfTimeToLayEgg, mostAmountOfTimeToLayEgg);
            }
        }

        if (name.Contains("Test")) 
            Debug.Log(string.Format("Range: {0} to {1}, Actual Timer: {2}", leastAmountOfTimeToLayEgg, mostAmountOfTimeToLayEgg, timer));

        Burn();

        if (hurt)
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D backInfo = Physics2D.Raycast(backDetection.position, Vector2.right, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        # region String Builder
        string air;
        string block;
        string ground;
        string back;

        bool NameCheck(RaycastHit2D hit2D, string name) {
            if (hit2D.collider.gameObject.name.Contains(name)) {
                return true;
            } else {
                return false;
            }
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

        if (backInfo.collider && backInfo.collider.tag != "NT" && backInfo.collider.tag != "Water" && !NameCheck(backInfo, "Gunflower") && !NameCheck(backInfo, "Rock")) {
            back = backInfo.collider.tag;
        } else {
            back = "Nothing";
        }

        if (groundInfo.collider) {
            ground = groundInfo.collider.tag;
        } else {
            ground = "Nothing";
        }

        #endregion

        if (FlipEnemyConstraints(ground, block, air, back, blockInfo)) 
            FlipEnemy();

        if ((block == "Ground" || block == "Bounce Pad" || block == "Block") && air == "Nothing") {
            Jump();
        }
    }

    bool FlipEnemyConstraints(string ground, string block, string air, string back, RaycastHit2D blockInfo) {
        return (ground == "Nothing" || block == "Player" || block == "Health Pot" || (block == "Enemy" && blockInfo.collider.gameObject != gameObject) || air == "Ground" || air == "Block") && (back != "Enemy" || back != "Player");
    }

    void FlipEnemy() {
        if (movingLeft == true) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = false;
        } else {
            transform.eulerAngles = new Vector3(0, 180, 0);
            movingLeft = true;
        }
    }

    void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
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
        currentHealth -= damage;
        int random = Mathf.RoundToInt(Random.Range(0, 100));
        hurt = true;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        if (name.Contains("Baby")) {
            HelperFunctions.IncrementInt("Baby Chickens Killed");
        } else {
            HelperFunctions.IncrementInt("Chickens Killed");
        }
        
        StartCoroutine(Blood());
        GenerateDrops();
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    void GenerateDrops() {
        if (name.Contains("Baby")) {
            if (burnTime > 0) {
                foreach (KeyValuePair<string, int> entry in babyBurnDrops) {
                    int random = Mathf.RoundToInt(Random.Range(0, 100));

                    if (random <= entry.Value) {
                        Drop(RemoveIntegers(entry.Key), 1);
                    }
                }
            } else {
                foreach (KeyValuePair<string, int> entry in babyDrops) {
                    int random = Mathf.RoundToInt(Random.Range(0, 100));

                    if (random <= entry.Value) {
                        Drop(RemoveIntegers(entry.Key), 1);
                    }
                }
            }
        } else {
            if (burnTime > 0) {
                foreach (KeyValuePair<string, int> entry in burnDrops) {
                    int random = Mathf.RoundToInt(Random.Range(0, 100));

                    if (random <= entry.Value) {
                        Drop(RemoveIntegers(entry.Key), 1);
                    }
                }
            } else {
                foreach (KeyValuePair<string, int> entry in drops) {
                    int random = Mathf.RoundToInt(Random.Range(0, 100));

                    if (random <= entry.Value) {
                        Drop(RemoveIntegers(entry.Key), 1);
                    }
                }
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

    string RemoveIntegers(string input) {
        return Regex.Replace(input, @"[\d-]", string.Empty);
    }

}
