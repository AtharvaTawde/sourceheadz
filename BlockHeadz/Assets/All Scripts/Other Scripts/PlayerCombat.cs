using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour {

    #region Public Variables

    public Animator animator;
    public Transform attackPoint;
    public Transform bottleDetection;
    public Transform keyDetection; 
    public Transform groundDetection;
    public Transform gateDetection;
    public Transform Explosion;
    public LayerMask goblins;
    public LayerMask enemyLayers;
    public LayerMask fallens;
    public LayerMask archers;
    public LayerMask keys;
    public int maxHealth = 200;
    public int currentHealth;
    public int atkdmg;
    public bool hasKey = false;
    public bool opened = false;
    public bool dead = false;
    public bool thruGate = false;
    public float attackRange = 1f;
    public float previousHealth;    
    public GameObject Gateway;
    #endregion
    
    #region Serizalized Variables

    [SerializeField] LayerMask itemLayer; 
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject itemsParent;
    [SerializeField] ItemSlot[] itemSlots;
    [SerializeField] GameObject Inventory;
    [SerializeField] int attackDamage = 20;
    [SerializeField] int weaponAtkIncrease;
    [SerializeField] static readonly float cooldown = .5f;
    #endregion

    #region Private Variables
    
    private float hitCooldown; 
    private int scene;
    private bool isGrounded, crouch;
    private bool takesMinorDamage;
    private bool takesMajorDamage;
    private float minimumHeight = 2f;
    private float jumpHeight = 0;
    private float damageReduction; 
    private string sceneName;
    private Rigidbody2D rb; 
    private CameraShake cameraShake;
    private GameObject[] accessories = new GameObject[8]; 
    private GameObject healthPlusParticleEffect;
    private GameObject microwave;
    private AudioSource audioSource;
    private AudioClip hitSound, nightAmbience;
    #endregion

    private void OnValidate() {
        if (itemsParent != null) 
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
    }

    void Start() {
        if (itemsParent != null)
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        
        hitCooldown = 0f;
        audioSource = GetComponent<AudioSource>();
        hitSound = Resources.Load("hit") as AudioClip;
        nightAmbience = Resources.Load("nightAmbience") as AudioClip;
        currentHealth = maxHealth;
        previousHealth = currentHealth;
        rb = GetComponent<Rigidbody2D>();
        scene = SceneManager.GetActiveScene().buildIndex;
        sceneName = SceneManager.GetActiveScene().name;
        healthPlusParticleEffect = Resources.Load("Plus Particles") as GameObject;
        microwave = GameObject.Find("Microwave");
        
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
        damageReduction = Inventory.GetComponent<EquipmentSystem>().damageReduction;
        weaponAtkIncrease = Inventory.GetComponent<EquipmentSystem>().weaponDamageIncrease;
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>(); 
        atkdmg = attackDamage + weaponAtkIncrease;    

        if (!GetComponent<PlayerMovement>().isPaused) {
            crouch = GetComponent<PlayerMovement>().crouch;
            isGrounded = GetComponent<CharacterController2D>().m_Grounded;

            FallDamage();

            if (Input.GetKeyDown(KeyCode.E)) {
                currentHealth += 25; //Replace with DrinkHP();
            }

            if (hitCooldown > 0f) {
                hitCooldown -= Time.deltaTime;
            } else if (hitCooldown <= 0f) {
                hitCooldown = 0f;
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && !crouch && hitCooldown <= 0f) {
                Attack(); 
                hitCooldown = cooldown;   
            }

            if (currentHealth <= 0)
                currentHealth = 0;

            if (hasKey) {
                Gateway.SetActive(true);
                opened = true;
            }

            AssignAccessories();
        }
    }

    # region Health Pots
    public void DrinkHP() {
        for (int i = 0; i < itemSlots.Length; i++) {
            if (itemSlots[i].Item != null && itemSlots[i].Item.ItemName == "Health Pot") {
                itemSlots[i].Amount--;
                StartCoroutine(HealthPotDrinkParticles());
                StartCoroutine(Drinky());
                break;
            }
        }
    }

    public IEnumerator Drinky() {
        for (int j = 0; j < 25; j++) {
            currentHealth += 1;
            if (currentHealth >= maxHealth) {
                currentHealth = maxHealth;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    IEnumerator HealthPotDrinkParticles() {
        GameObject plusInstance = Instantiate(healthPlusParticleEffect, new Vector3(transform.position.x, transform.position.y + 0.87f - 2f, transform.position.z), transform.rotation);
        yield return new WaitForSeconds(2f);
        Destroy(plusInstance);
    }
    #endregion

    public void TakeDamage(int damage) {
        currentHealth -= Mathf.RoundToInt(damage * (1 - damageReduction));
        
        if (currentHealth <= 0) {
            Instantiate(Explosion, transform.position, transform.rotation);
            PlayerDie();
        }
    }

    public void PlayerDie() {
        currentHealth = 0;
        dead = true;
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, .01f);
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
            TakeDamage(Mathf.RoundToInt(jumpHeight * Random.Range(20f, 25f)));
            takesMinorDamage = false;
            StartCoroutine(cameraShake.Shake(.15f, .75f));
        } else if (takesMajorDamage && isGrounded) {
            TakeDamage(Mathf.RoundToInt(jumpHeight * Random.Range(25.1f, 30f)));
            takesMajorDamage = false;
            StartCoroutine(cameraShake.Shake(.30f, 1f));
        }

        if (isGrounded) {jumpHeight = 0;}
        
        if (currentHealth <= 0) {
            currentHealth = 0;
            PlayerDie();
        }
    }

    public void Attack() {
        animator.SetTrigger("Attack");
        float random = Random.Range(.5f, 1f);
        audioSource.PlayOneShot(hitSound, random);
        StartCoroutine(cameraShake.Shake(.15f, .05f));

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        Collider2D[] hitGoblins = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, goblins);
        Collider2D[] hitFallens = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, fallens);
        Collider2D[] hitArchers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, archers);
        Collider2D[] hitKeys = Physics2D.OverlapCircleAll(keyDetection.position, attackRange, keys);
        Collider2D[] hitItem = Physics2D.OverlapCircleAll(bottleDetection.position, attackRange, itemLayer);  
        
        foreach(Collider2D enemy in hitEnemies) {
            enemy.GetComponent<Enemy>().TakeDamage(atkdmg);
            enemy.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }

        foreach(Collider2D goblin in hitGoblins) {
            goblin.GetComponent<GoblinEnemy>().TakeDamage(atkdmg);
            goblin.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }

        foreach(Collider2D fallen in hitFallens) {
            fallen.GetComponent<FallenCombat>().TakeDamage(atkdmg);
            fallen.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }  

        foreach(Collider2D archer in hitArchers) {
            archer.GetComponent<ArcherCombat>().TakeDamage(atkdmg);
            archer.GetComponent<Rigidbody2D>().AddForce(new Vector2(250, 500));
        }   

        foreach(Collider2D key in hitKeys) {
            Destroy(key.gameObject);
            hasKey = true; 
        }

        string[] itemNames = {"Health Pot", "Goblin Meat", "Nyert Meat", "Wood Shard", "Eyeball", "Vest", "Stone"}; 
        foreach (Collider2D item in hitItem) {
            Item itemCopy;
            for (int i = 0; i < itemNames.Length; i++) {
                if (item.gameObject.name.Contains(itemNames[i])) {
                    itemCopy = (Resources.Load("Items/" + itemNames[i]) as Item).GetCopy();  
                
                    if (inventory.AddItem(itemCopy)) {
                        Destroy(item.gameObject);
                    } 
                } 
            }
        }     
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "World Boundary" || other.gameObject.name == "Death") {
            PlayerDie();
        } 
        
        if (other.gameObject.tag == "Bartie") {
            PlayerPrefs.SetString("Visit Bartie", "True");
        }

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

        if (PlayerPrefs.GetInt("Goblins Killed") + PlayerPrefs.GetInt("Zombies Killed") + PlayerPrefs.GetInt("Archers Killed") >= 1000 && PlayerPrefs.GetString("Item5") != "True") 
            PlayerPrefs.SetString("Item5", "True");

        if (PlayerPrefs.GetString("Visit Bartie") == "True" && PlayerPrefs.GetString("Item7") != "True") 
            PlayerPrefs.SetString("Item7", "True");
    }
}       