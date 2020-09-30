using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{

    #region Transforms
    public Animator animator;
    public Transform attackPoint;
    public Transform bottleDetection;
    public Transform keyDetection; 
    public Transform groundDetection;
    public Transform gateDetection;
    
    #endregion
    
    #region Layer Masks
    public LayerMask enemyLayers;
    public LayerMask goblins;
    public LayerMask fallens;
    public LayerMask healthpotions; 
    public LayerMask keys;
    public LayerMask limit;
    public LayerMask gateway;
    #endregion

    #region Necessary Stuff
    public int maxHealth = 200;
    public int currentHealth;
    public int HPOTS = 0;
    public int attackDamage = 20;
    private int scene;
    private bool isGrounded, crouch;
    private bool takesMinorDamage;
    private bool takesMajorDamage;
    public bool hasKey = false;
    public bool opened = false;
    public bool dead = false;
    public bool thruGate = false;
    public float attackRange = 0.5f;
    private float minimumHeight = 2f;
    private float jumpHeight = 0;
    public float previousHealth;
    private string sceneName;
    public Transform Explosion;
    public GameObject Gateway;  
    private Rigidbody2D rb;
    
    #endregion

    #region Scene 10 Gameobjects
    GameObject Guards;   
    GameObject ExitGuards;
    GameObject[] accessories = new GameObject[6]; // change the # everytime a new item is added

    #endregion

    void Start() {
        currentHealth = maxHealth;
        previousHealth = currentHealth;
        rb = GetComponent<Rigidbody2D>();
        scene = SceneManager.GetActiveScene().buildIndex;
        sceneName = SceneManager.GetActiveScene().name;
        Guards = GameObject.Find("Guards");
        ExitGuards = GameObject.Find("Exit Guards");
        
        for (int i = 0; i < accessories.Length; i++) {
            accessories[i] = GameObject.Find("Player/Accessory Container/Item" + i);
            if (accessories[i].name == PlayerPrefs.GetString("Selected Item")) {
                accessories[i].SetActive(true);
            } else {
                accessories[i].SetActive(false);
            }
        }
    }

    void Update() {
        if (!GetComponent<PlayerMovement>().isPaused) {
            crouch = GetComponent<PlayerMovement>().crouch;
            isGrounded = GetComponent<CharacterController2D>().m_Grounded;

            if (scene != 10)
                FallDamage();

            DrinkHP();

            if (Input.GetKeyDown(KeyCode.Space) && !crouch)
                Attack(); 

            if (currentHealth <= 0)
                currentHealth = 0;

            if (hasKey && scene != 10) {
                Gateway.SetActive(true);
                opened = true;
            } else if (scene == 10) {
                if (Guards.activeSelf && hasKey) {
                    Guards.SetActive(false);
                } else if (!Guards.activeSelf && ExitGuards.activeSelf && hasKey) {
                    ExitGuards.SetActive(false);
                }
            }

            AssignAccessories();
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
        dead = true;
        Transform bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as Transform;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, .5f);
        Destroy(bloodInstance.gameObject, 10f);
    }

    public void FallDamage() {
        if (!isGrounded) {
            jumpHeight += Time.deltaTime;
        }

        if (jumpHeight >= minimumHeight && jumpHeight <= minimumHeight + 2) {
            takesMinorDamage = true;
        } else if (jumpHeight >= minimumHeight + 2) {
            takesMajorDamage = true;
        }

        if (takesMinorDamage && isGrounded) {
            TakeDamage(Mathf.RoundToInt(jumpHeight * Random.Range(10f, 15f)));
            takesMinorDamage = false;
        } else if (takesMajorDamage && isGrounded) {
            TakeDamage(Mathf.RoundToInt(jumpHeight * Random.Range(15.1f, 20f)));
            takesMajorDamage = false;
        }

        if (isGrounded) {
            jumpHeight = 0;
        }

        if (currentHealth <= 0) {
            currentHealth = 0;
            PlayerDie();
        }
    }

    public void Attack() {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitGoblins = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, goblins);
        Collider2D[] hitFallens = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, fallens);
        Collider2D[] hitHPots = Physics2D.OverlapCircleAll(bottleDetection.position, attackRange, healthpotions);
        Collider2D[] hitKeys = Physics2D.OverlapCircleAll(keyDetection.position, attackRange, keys);
        
        foreach(Collider2D enemy in hitEnemies) {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }
        foreach(Collider2D goblin in hitGoblins) {
            goblin.GetComponent<GoblinEnemy>().TakeDamage(attackDamage);
            goblin.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }
        foreach(Collider2D fallen in hitFallens) {
            fallen.GetComponent<FallenCombat>().TakeDamage(attackDamage);
            fallen.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
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

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "World Boundary" || other.gameObject.name == "Death") {
            PlayerDie();
        }    
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Gateway") {
            opened = false;
            hasKey = false;
            thruGate = true;
        }    
    }

    void AssignAccessories() {
        if (sceneName == "Stage 2" && PlayerPrefs.GetString("Item1") != "True")
            PlayerPrefs.SetString("Item1", "True");

        if (sceneName == "Stage 3" && PlayerPrefs.GetString("Item2") != "True")
            PlayerPrefs.SetString("Item2", "True");

        if (sceneName == "FinalStage" && PlayerPrefs.GetString("Item3") != "True")
            PlayerPrefs.SetString("Item3", "True");

        if (sceneName == "GameComplete" && PlayerPrefs.GetString("Item4") != "True") 
            PlayerPrefs.SetString("Item4", "True");

        if (PlayerPrefs.GetInt("Goblins Killed") + PlayerPrefs.GetInt("Zombies Killed") >= 1000 && PlayerPrefs.GetString("Item5") != "True") 
            PlayerPrefs.SetString("Item5", "True");
    }
}       
