using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
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

    // Layermasks
    [SerializeField] LayerMask enemies;
    [SerializeField] LayerMask projectiles;
    [SerializeField] LayerMask plants;
    [SerializeField] LayerMask animals;
    [SerializeField] LayerMask bosses;
    [SerializeField] LayerMask triggerBlocks;
    [SerializeField] LayerMask obstacles;
    [SerializeField] LayerMask itemLayer; 
    [SerializeField] LayerMask inAnimateLayer;
    
    public float burnTime = 0;
    public float maxHunger = 200;
    public float saturation = 5;
    public float currentHunger;
    public int maxHealth = 200;
    public int currentHealth;
    public int displayHunger;
    public int deaths = 0;
    public int atkdmg;
    public int regenerationConstant;
    public bool heal;
    public GameObject Gateway;
    public GameObject[] playerComponents = new GameObject[9];
    public RaycastHit2D hit;
    #endregion
    
    #region Serizalized Variables
    [SerializeField] int weaponAtkIncrease;
    [SerializeField] Inventory inventory;
    [SerializeField] CraftingSystem craftingSystem;
    [SerializeField] GameObject Inventory;
    [SerializeField] GameObject onFireGraphic;
    [SerializeField] GameObject healthPot;
    [SerializeField] GameObject itemsParent;
    [SerializeField] ItemSlot[] itemSlots;
    [SerializeField] static readonly float cooldown = .5f;
    #endregion

    #region Private Variables
    private string hoveringOver;
    private string savedBlock;
    private float attackRange = 1.5f;
    private float hitCooldown;  
    private float saturationHealTime;                
    private float burnCooldown = 0f; 
    private float damageReduction; 
    private float randomPitch;
    private float fallTime = 0;
    public float breakTime;
    private int attackDamage = 50;
    private int scene;  
    private int hostileMobsKilled;
    private bool isGrounded, crouch;
    private bool negateFallDamage;
    private bool facingRight;    
    private bool hurt = false;
    private bool hasFallen = false;
    private Rigidbody2D rb; 
    private CameraShake cameraShake;
    private GameObject[] accessories = new GameObject[8]; 
    private GameObject healthPlusParticleEffect;
    private AudioSource audioSource;
    private AudioClip hitSound, hurtSound, submergeSound, emergeSound;
    private Vector3 spawnPoint;
    private Vector3 realPoint;
    private BlockIndicator currentBlock; 
    #endregion

    private void OnValidate() {
        if (itemsParent != null) {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        }
    }

    void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 300;
        if (itemsParent != null) {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        }

        for (int i = 0; i < transform.childCount; i++) {
            playerComponents[i] = transform.GetChild(i).gameObject;
        }

        currentHealth = maxHealth;
        currentHunger = maxHunger;
        saturationHealTime = 2.5f;
        hitCooldown = 0f;
        burnCooldown = 0f;
        audioSource = GetComponent<AudioSource>();
        hitSound = Resources.Load("hit") as AudioClip;
        hurtSound = Resources.Load("hurt") as AudioClip;
        submergeSound = Resources.Load("submerge") as AudioClip;
        emergeSound = Resources.Load("emerge") as AudioClip;
        rb = GetComponent<Rigidbody2D>();
        scene = SceneManager.GetActiveScene().buildIndex;
        healthPlusParticleEffect = Resources.Load("Plus Particles") as GameObject;

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
        displayHunger = Mathf.RoundToInt(currentHunger);
        facingRight = GetComponent<CharacterController2D>().m_FacingRight;
        damageReduction = Inventory.GetComponent<EquipmentSystem>().damageReduction;
        weaponAtkIncrease = Inventory.GetComponent<EquipmentSystem>().weaponDamageIncrease;
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>(); 
        randomPitch = Random.Range(.5f, 1f);
        atkdmg = attackDamage + weaponAtkIncrease;    
        hostileMobsKilled = PlayerPrefs.GetInt("Goblins Killed") + 
                            PlayerPrefs.GetInt("Zombies Killed") + 
                            PlayerPrefs.GetInt("Archers Killed") + 
                            PlayerPrefs.GetInt("Revenants Killed") + 
                            PlayerPrefs.GetInt("Firetraps Killed") + 
                            PlayerPrefs.GetInt("Fallens Killed");

        if (!GetComponent<PlayerMovement>().isPaused && !GetComponent<PlayerMovement>().isDead) {
            crouch = GetComponent<PlayerMovement>().crouch;
            isGrounded = GetComponent<CharacterController2D>().m_Grounded;
            float k_foodExhaustion = 200f * 1f / Convert.ToSingle(currentHealth);

            spawnPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            realPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
            hit = Physics2D.Raycast(realPoint, Vector2.zero);

            // Check the exact Gameobject the cursor is on 
            hoveringOver = hit.collider != null ? hoveringOver = hit.collider.name : hoveringOver = "Nothing";
            saturation = saturation > 0 ? saturation -= Time.deltaTime : saturation = 0; 

            if (Mathf.Abs(GetComponent<PlayerMovement>().horizontalMove) > 60f && saturation == 0) {
                currentHunger -= Time.deltaTime * k_foodExhaustion;
            } else if (Mathf.Abs(GetComponent<PlayerMovement>().horizontalMove) > 30f && saturation == 0) {
                currentHunger -= Time.deltaTime * k_foodExhaustion / 2; 
            }

            // Auto regen if hunger > 80%
            if (currentHunger / maxHunger > 0.8f) {
                if (saturationHealTime > 0) {
                    saturationHealTime -= Time.deltaTime;
                } else {
                    saturationHealTime = 0;
                    currentHealth += 5;
                    
                    if (saturation > 0) {
                        saturation -= 1f;
                    } else {
                        currentHunger -= 1f;
                    }
                    
                    saturationHealTime = 2.5f;
                }
            } else {
                saturationHealTime = 2.5f;
            }

            if (!craftingSystem.isCraftingMenuActive) {
                LeftClickLogistics();
            }
            
            Burn();

            if (!negateFallDamage) {
                FallDamage();
                negateFallDamage = false;
            } else {
                fallTime = 0;
            }

            if (hurt)
                StartCoroutine(GetComponent<HitEffect>().HurtEffect());
                hurt = false;

            if (currentHealth <= 0)
                currentHealth = 0;
            
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            
            if (currentHunger <= 0)
                currentHunger = 0;
            
            if (currentHunger > maxHunger)
                currentHunger = maxHunger;

            AssignAccessories();
        }
    }

    public IEnumerator HealthPotDrinkParticles() {
        GameObject plusInstance = Instantiate(healthPlusParticleEffect, new Vector3(transform.position.x, transform.position.y + 0.87f - 2f, transform.position.z), transform.rotation);
        yield return new WaitForSeconds(2f);
        Destroy(plusInstance);
    }

    public void TakeDamage(int damage) {
        audioSource.PlayOneShot(hurtSound, randomPitch);
        hurt = true;
        currentHealth -= Mathf.RoundToInt(damage * (1 - damageReduction));
        saturation -= 0.05f * damage;
        if (currentHealth <= 0) {
            Instantiate(Explosion, transform.position, transform.rotation);
            audioSource.PlayOneShot(hitSound, randomPitch);
            PlayerDie();
        }
    }

    public void PlayerDie() {
        foreach (ItemSlot itemSlot in itemSlots) {
            float throwPower = Random.Range(0, 500);
            if (itemSlot.Item != null && itemSlot.Item.ItemName != "Health Pot") {
                GameObject item = Instantiate(Resources.Load("Physical Items/" + itemSlot.Item.ItemName) as GameObject, transform.position, transform.rotation);
                item.gameObject.name = string.Format("{0}x{1}", itemSlot.Item.ItemName, itemSlot.Amount);
                GetComponent<ChunkLoad>().items.Add(item);
                if (GetComponent<CharacterController2D>().m_FacingRight) {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                } else {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                }
            } else if (itemSlot.Item != null && itemSlot.Item.ItemName == "Health Pot") {
                for (int i = 0; i < itemSlot.Amount; i++) {
                    GameObject item = Instantiate(healthPot, transform.position, transform.rotation);
                    item.gameObject.name = "Health Potx1";
                    if (GetComponent<CharacterController2D>().m_FacingRight) {
                        item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                    } else {
                        item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                    }
                }
            }

            itemSlot.Item = null;
            itemSlot.Amount = 0;
        }

        currentHealth = 0;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<PlayerMovement>().isDead = true;
        foreach (GameObject g in playerComponents) {    
            g.SetActive(false);
        }

        burnTime = 0f;
        fallTime = 0f;
        GetComponent<ItemSelection>().eatTime = 0f;
        deaths += 1;

    }

    public void FallDamage() {
        if (rb.velocity.y < -0.5f) {
            fallTime += Time.deltaTime;
            hasFallen = true;
        } else if (hasFallen) {
            if (fallTime > .75f) {
                TakeDamage(Mathf.RoundToInt(fallTime * 20f));
                StartCoroutine(cameraShake.Shake(.30f, 1f));
            }

            hasFallen = false;
            fallTime = 0;
        }
    }

    public void Attack() {
        animator.SetTrigger("Attack");
        audioSource.PlayOneShot(hitSound, randomPitch);
        StartCoroutine(cameraShake.Shake(.15f, .05f));

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemies);
        Collider2D[] hitPlants = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, plants);
        Collider2D[] hitAnimals = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, animals);
        Collider2D[] hitBosses = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, bosses);
        Collider2D[] hitProjectiles = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, projectiles);
        Collider2D[] hitInAnimateObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, inAnimateLayer);  
        Collider2D[] hitItem = Physics2D.OverlapCircleAll(bottleDetection.position, attackRange, itemLayer);  
        
        Collider2D[][] colliders = {hitEnemies, hitPlants, hitAnimals};
        
        // Mobs (Animals and Enemies)
        foreach (Collider2D[] set in colliders) {
            foreach(Collider2D collider in set) {
                collider.GetComponent<TakeDamage>().ReceiveDamage(atkdmg);
                if (collider.GetComponent<Rigidbody2D>() != null) {
                    Vector3 hitDirection = transform.position - collider.transform.position;
                    float _mass = collider.GetComponent<Rigidbody2D>().mass;
                    collider.GetComponent<Rigidbody2D>().AddForce(-hitDirection * 175 * _mass);  
                    collider.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 175 * _mass);                  
                }    
            }
        }

        // Bosses
        foreach(Collider2D collider in hitBosses) {
            collider.GetComponent<TakeDamage>().ReceiveDamage(atkdmg);
        }

        // Projectiles
        foreach (Collider2D projColl in hitProjectiles) {
            Destroy(projColl.gameObject);
        }

        // In World, Static Objects (Not Plants or Segmented Entities)
        foreach (Collider2D c in hitInAnimateObjects)  {
            c.GetComponent<InanimateObject>().DestroySelf();   
        }

        // Item Pickup
        string[] itemNames = {"Apple", "Baton", "Blood Pie", "Blue Crystal", "Boss Gem", "Bread Scrap", "Chicken", "Clay Idol", 
                              "Clay Slab", "Coal Dust", "Cooked Chicken", "Cooked Goblin Meat", "Cooked Nyert Meat", 
                              "Cooked Zombie Spleen", "Corpuscle", "Diamond", "Diamond Boots", "Diamond Chestplate", 
                              "Diamond Leggings", "Diamond Sword", "Egg", "Empty Bottle", "Eyeball", "Fabric", "Fireball", "Glass Shard", 
                              "Goblin Meat", "Gold Bar", "Gold Scrap", "Green Crystal", "Gunflower Node", "Gunflower Seed", 
                              "Health Pot", "Jump Potion", "Nyert Meat", "Oven", "Popper", "Red Crystal", "Speed Potion", 
                              "Stick", "Stone", "Stone Block", "Stone Boots", "Stone Chestplate", "Stone Leggings", "Stone Sword", 
                              "Stripped Log", "Titanium Bar", "Titanium Boots", "Titanium Chestplate", "Titanium Leggings", "Titanium Nugget", 
                              "Titanium Sword", "Tungsten Carbide Boots", "Tungsten Carbide Chestplate", "Tungsten Carbide Leggings", 
                              "Tree Trunk", "Varium", "Varium Boots", "Varium Chestplate", "Varium Leggings", 
                              "Varium Sword", "Water Bottle", "Wood Shard", "Yellow Crystal", "Zombie Spleen"};

        foreach (Collider2D item in hitItem) {
            List<string> check = new List<string>();
            for (int i = 0; i < itemNames.Length; i++) {
                string[] info = item.gameObject.name.Split('x');
                if (info[0] == itemNames[i]) {
                    Item itemCopy = (Resources.Load("Items/" + itemNames[i]) as Item);  
                    for (int j = 0; j < Int32.Parse(info[1]); j++) {
                        if (inventory.AddItem(itemCopy)) {
                            check.Add("");
                        }
                    }

                    if (check.Count > 0) {
                        Destroy(item.gameObject);
                        check.Clear();
                        break;
                    }
                } 
            }
        }     
    }

    void Break() {
        audioSource.PlayOneShot(hitSound, randomPitch);

        // All Blocks
        float blockClickable = Vector3.Distance(realPoint, hit.collider.transform.position);  
        if (blockClickable <= 0.5f) {
            hit.collider.GetComponent<TakeDamage>().BlockDrop();    
        }    
    }

    void LeftClickLogistics() {
        hitCooldown = hitCooldown > 0 ? hitCooldown -= Time.deltaTime : hitCooldown = 0;

        // If not holding left-click, reset breakTime
        if (!Input.GetMouseButton(0) && CurrentBlockName() != "Nothing") {
            breakTime = hit.collider.GetComponent<BlockIndicator>().blockBreakTime;
            hit.collider.GetComponent<BlockIndicator>().crackAnimator.SetBool("Break", false);
            savedBlock = CurrentBlockName();
        }

        // If saved block is not equal to the current block, reset breakTime
        if (savedBlock != CurrentBlockName()) {
            if (currentBlock != null) {
                breakTime = currentBlock.blockBreakTime;
                currentBlock.crackAnimator.SetBool("Break", false);
            }   
            savedBlock = CurrentBlockName();    
        }

        // Select Between Attacking and Breaking
        if (!HelperFunctions.IsPointerOverUIElement()) {
            if (!crouch && hitCooldown <= 0f && Input.GetMouseButtonDown(0) && CurrentBlockName() == "Nothing") {

                Attack(); 
                saturation -= 0.05f;
                hitCooldown = cooldown; 

            } else if (Input.GetMouseButton(0) && CurrentBlockName() != "Nothing") {
                
                if (breakTime > 0) {    
                    breakTime -= Time.deltaTime;
                    currentBlock = hit.collider.GetComponent<BlockIndicator>();
                    currentBlock.crackAnimator.SetFloat("Playback Speed", 1 / hit.collider.GetComponent<BlockIndicator>().blockBreakTime);
                    currentBlock.crackAnimator.SetBool("Break", true);
                } else if (breakTime <= 0) {
                    breakTime = 0;
                    Break();
                    breakTime = hit.collider.GetComponent<BlockIndicator>().blockBreakTime;
                    currentBlock = null;
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

    string CurrentBlockName() {
        if (hit.collider != null && hit.collider.GetComponent<BlockIndicator>() != null) {
            return hit.collider.GetComponent<BlockIndicator>().n;
        } else {
            return "Nothing";
        }
    }

    #region Collision/Trigger Stuff
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name.Contains("World Boundary") || other.gameObject.name == "Death") {
            PlayerDie();
        }   
        
        if (other.gameObject.tag == "Bartie") {
            PlayerPrefs.SetString("Visit Bartie", "True");
        }

        if (other.gameObject.tag == "Gateway") {
            if (other.transform.localScale == Vector3.one * 2) {
                transform.position = new Vector3(957f, 598f, 0f);
            }
        }

        if (other.gameObject.tag == "Water") {
            burnTime = 0f;
            audioSource.PlayOneShot(submergeSound, randomPitch);
            negateFallDamage = true;
            GetComponent<PlayerMovement>().runSpeed = 15f;
            rb.gravityScale = -2f;
        }

        if (other.gameObject.name.Contains("Obsidian Dropdown")) {
            StartCoroutine(MakeDropdownFall(other.gameObject));
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Water") {
            burnTime = 0f;
            negateFallDamage = true;
            GetComponent<PlayerMovement>().runSpeed = 15f;
            rb.gravityScale = -2f;
        }

        if (other.gameObject.tag == "Gateway") {
            if (other.transform.localScale == Vector3.one * 2) {
                transform.position = new Vector3(961f, 1450f, 0f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Water") {
            audioSource.PlayOneShot(emergeSound, randomPitch);
            negateFallDamage = false;
            rb.gravityScale = 1f;
        }     
    }

    IEnumerator MakeDropdownFall(GameObject g) {
        g.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(1.5f);
        g.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.name == "Bounce") {
            negateFallDamage = true;
        }
    }
    #endregion

    void AssignAccessories() { // change all the conditions
        if (hostileMobsKilled >= 100 && PlayerPrefs.GetString("Item1") != "True")
            PlayerPrefs.SetString("Item1", "True");

        if (hostileMobsKilled >= 500 && PlayerPrefs.GetString("Item2") != "True")
            PlayerPrefs.SetString("Item2", "True");

        if (hostileMobsKilled >= 1000 && PlayerPrefs.GetString("Item3") != "True")
            PlayerPrefs.SetString("Item3", "True");

        if (hostileMobsKilled >= 5000 && PlayerPrefs.GetString("Item4") != "True") 
            PlayerPrefs.SetString("Item4", "True");

        if (hostileMobsKilled >= 10000 && PlayerPrefs.GetString("Item5") != "True")
            PlayerPrefs.SetString("Item5", "True");

        if (GetComponent<ItemSelection>().portalActivated && PlayerPrefs.GetString("Item6") != "True") 
            PlayerPrefs.SetString("Item6", "True");
        
        if (PlayerPrefs.GetString("Visit Bartie") == "True" && PlayerPrefs.GetString("Item7") != "True") 
            PlayerPrefs.SetString("Item7", "True");
    }

}       