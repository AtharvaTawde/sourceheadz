using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class Traveler : MonoBehaviour  {
    // Put NT tag on anything that should not be detected by a Nyert!!!
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    [SerializeField] Transform travelerBody;
    [SerializeField] Animator animator;
    [SerializeField] GameObject onFireGraphic;
    [SerializeField] GameObject heldItem;  
    [SerializeField] LayerMask itemLayer; 
    [SerializeField] float speed;
    
    private Rigidbody2D rb;
    private GameObject Explosion;    
    private Sprite heldItemSprite;
    private float viewFinder;
    private float jumpForce;
    private float burnCooldown = 0f;
    private bool movingLeft = true;
    private bool hurt;
    private int maxHealth = 350;
    private int currentHealth;

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"", 1}
        //{"Fabric1", 100},
        //{"Fabric2", 50},
        //{"Fabric3", 25},
        //{"Fabric4", 12},
        //{"Fabric5", 6}
    };

    private Dictionary<string, int> burnDrops = new Dictionary<string, int> {
        {"", 1}
        //{"Fabric1", 100},
        //{"Fabric2", 50},
        //{"Fabric3", 25},
        //{"Fabric4", 12},
        //{"Fabric5", 6}
    };

    private List<string> barterableItems = new List<string> {
        "Stick",
        "Glass Shard",
        "Coal Dust",
        "Clay Slab",
        "Eyeball",
        "Gold Scrap",
        "Gunflower Node",
        "Gunflower Seed",
        "Stone",
        "Stripped Log",
        "Tree Trunk",
        "Titanium Bar",
        "Titanium Nugget",
        "Wood Shard",
        "Popper"
    };

    public float burnTime = 0f;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = transform.GetChild(0);
        blockDetection = transform.GetChild(1);
        airDetection = transform.GetChild(2);
        jumpForce = Mathf.Sqrt(Physics2D.gravity.magnitude * rb.gravityScale / 16) * rb.mass;
        Explosion = Resources.Load("Blood") as GameObject;
        currentHealth = maxHealth;
    }

    void Update() {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        viewFinder = movingLeft ? .4f : -.4f;

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        Burn();

        if (hurt) {
            foreach (Transform bodyPart in travelerBody) {
                StartCoroutine(bodyPart.GetComponent<HitEffect>().HurtEffect());
            }
            hurt = false;
        }

        # region String Builder

        bool NameCheck(RaycastHit2D hit2D, string name) {
            if (hit2D.collider.gameObject.name.Contains(name)) {
                return true;
            } else {
                return false;
            }
        }  

        bool ColliderCheck(RaycastHit2D hit) {
            return hit.collider && 
                   hit.collider.tag != "NT" && 
                   hit.collider.tag != "Water" && 
                   !NameCheck(hit, "Gunflower") && 
                   !NameCheck(hit, "Rock") && 
                   !NameCheck(hit, "Health Pot") && 
                   hit.collider.tag != "Item";
        }

        string Air() {
            if (ColliderCheck(airInfo)) {
                return airInfo.collider.tag;  
            } else {
                return "Nothing";
            }
        }

        string Block() {
            if (ColliderCheck(blockInfo)) {
                return blockInfo.collider.tag;
            } else {
                return "Nothing";
            }
        }

        string Ground() {
            if (groundInfo.collider) {
                return groundInfo.collider.tag;
            } else {
                return "Nothing";
            }
        }

        #endregion

        if (Ground() == "Nothing" || Air() == "Block" || (Block() == "Enemy" && blockInfo.collider.gameObject != gameObject) || Air() == "Ground") {
            FlipEnemy();
        }

        if ((Block() == "Ground" || Block() == "Bounce Pad" || Block() == "Block") && Air() == "Nothing") {
            Jump();
        }

        if (Block() == "Player") {
            speed = 0; 
            animator.SetBool("Stop", true);
        } else {
            speed = 1.5f;
            animator.SetBool("Stop", false);
        }

        foreach (Collider2D i in Physics2D.OverlapCircleAll(transform.position, 1, itemLayer)) {
            if (i.gameObject.tag == "Item" && heldItem.GetComponent<SpriteRenderer>().sprite == null) {
                StartCoroutine(Barter(i));
            }
        }

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

    private void OnDrawGizmos() {
        Vector3 blockEndPoint = blockDetection.position;
        if (movingLeft) { 
            blockEndPoint.x -= viewFinder; 
        } else if (!movingLeft) {
            blockEndPoint.x += viewFinder;
        }

        Gizmos.DrawLine(blockDetection.position, blockEndPoint); 
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
        HelperFunctions.IncrementInt("Travelers Killed");
        StartCoroutine(Blood());
        GenerateDrops();
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    IEnumerator Barter(Collider2D pickup) {
        string[] info = pickup.gameObject.name.Split('x');
        string yourItem = info[0];
        if (TravelerWantsItem(yourItem)) {
            int amount = Int32.Parse(info[1]);

            heldItemSprite = pickup.gameObject.GetComponent<SpriteRenderer>().sprite;

            if (amount > 1) {
                pickup.gameObject.name = string.Format("{0}x{1}", yourItem, amount - 1);
            } else {
                Destroy(pickup.gameObject);
            }

            heldItem.GetComponent<SpriteRenderer>().sprite = heldItemSprite;
            
            yield return new WaitForSeconds(Random.Range(3, 5));

            string travelerItem = barterableItems[Random.Range(0, barterableItems.Count - 1)];
            GameObject item = Instantiate(Resources.Load(string.Format("Physical Items/{0}", travelerItem)) as GameObject, transform.position, transform.rotation);
            item.gameObject.name = string.Format("{0}x{1}", travelerItem, Random.Range(1, 6));

            if (transform.eulerAngles == Vector3.zero) {
                item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 350f);
            } else {
                item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 350f);
            }

            heldItem.GetComponent<SpriteRenderer>().sprite = null; 
        }
    }

    bool TravelerWantsItem(string itemName) {
        string[] wantedItems = {"Fabric", "Water Bottle", 
                                "Blood Pie", "Goblin Meat", "Nyert Meat", 
                                "Apple", "Bread Scrap", "Cooked Goblin Meat", 
                                "Cooked Nyert Meat", "Zombie Spleen", 
                                "Cooked Zombie Spleen", "Chicken", "Cooked Chicken"};

        foreach (string item in wantedItems) {
            if (item == itemName) {
                return true;
            }
        }
        return false;
    }

    void GenerateDrops() {
        if (burnTime > 0) {
            foreach (KeyValuePair<string, int> entry in burnDrops) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= entry.Value) {
                    Drop(entry.Key.RemoveIntegers(), 1);
                }
            }
        } else {
            foreach (KeyValuePair<string, int> entry in drops) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= entry.Value) {
                    Drop(entry.Key.RemoveIntegers(), 1);
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
    
}
