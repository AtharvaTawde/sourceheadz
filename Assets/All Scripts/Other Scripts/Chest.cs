using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {
    
    public bool isChestOpen;
    public ItemSlot[] chestSlots;
   
    [SerializeField] int currentHealth;
    [SerializeField] Sprite specialChestSprite;
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    [SerializeField] CraftingSystem c;
    [SerializeField] GameObject[] chestObjects;
    [SerializeField] Transform chestSlotsParent;
    [SerializeField] Transform player;
    
    private Vector3 spawnPoint;
    private Vector3 realPoint;
    private string hoveringOver;
    private bool hoverOverSelf;
    private bool isTGA;
    private bool isTGB;
    private bool isCheckComplete;
    private bool hasChestBeenFilled;
    private int maxHealth = 150;

    private Dictionary<string, float> regularDrops = new Dictionary<string, float> {
        {"Diamond",         45},
        {"Titanium Bar",    85},
        {"Gold Scrap",      450},
        {"Crimson Leaf",    475},
        {"Fabric",          500},
        {"Empty Bottle",    550},
        {"Corpuscle",       650},
        {"Clay Idol",       750},
        {"Clay Slab",       850},
        {"Coal Dust",       950}
    };

    private Dictionary<string, float> specialDrops = new Dictionary<string, float> {
        {"Varium Chestplate",               2},
        {"Varium Sword",                    3.5f},
        {"Varium Leggings",                 5},
        {"Varium Boots",                    6},
        {"Diamond Chestplate",              7},
        {"Diamond Sword",                   10},
        {"Diamond Leggings",                15},
        {"Diamond Boots",                   20},
        {"Titanium Chestplate",             60},
        {"Titanium Sword",                  100},
        {"Titanium Leggings",               125},
        {"Titanium Boots",                  250},
        {"Varium",                          275},
        {"Tungsten Carbide Chestplate",     600},
        {"Tungsten Carbide Leggings",       700},
        {"Tungsten Carbide Boots",          800}
    };

    private void Start() {
        currentHealth = maxHealth;

        if (!isCheckComplete) {
            if (IsOverlapping()) {
                Destroy(gameObject);
            }
            isCheckComplete = true;    
        }

        int state = Random.Range(1, 100);
        if (!gameObject.name.Contains("Special Chest")) {
            if (state < 15) {
                gameObject.name = "Special Chest";
                GetComponent<SpriteRenderer>().sprite = specialChestSprite;
            } else {
                gameObject.name = "Chest";
            }
        } else {
            gameObject.name = "Special Chest";
            GetComponent<SpriteRenderer>().sprite = specialChestSprite;
        }

        c = GameObject.Find("Canvas/Inventory").GetComponent<CraftingSystem>();
        player = GameObject.Find("Player").transform;

        if (chestSlotsParent != null) {
            chestSlots = chestSlotsParent.GetComponentsInChildren<ItemSlot>();
        }

        foreach (GameObject g in chestObjects) {
            g.transform.localScale = Vector3.zero;
        }
    }

    void SetItems() {
        int itemsAdded = 0;
        for (int i = 0; i < chestSlots.Length; i++) {
            if (!gameObject.name.Contains("Special Chest")) {
                
                int random = Mathf.RoundToInt(Random.Range(0, 1000));

                if (itemsAdded == 3)
                    break; 

                if (Random.Range(0, 100) < 50) {
                    foreach (KeyValuePair<string, float> entry in regularDrops) {
                        if (random <= entry.Value) {
                            chestSlots[i].Item = Resources.Load("Items/" + entry.Key) as Item;
                            chestSlots[i].Amount = Random.Range(1, 2);
                            itemsAdded += 1;
                            break; 
                        }
                    }
                }

            } else {
                
                int random = Mathf.RoundToInt(Random.Range(0, 1000));

                if (itemsAdded == 2)
                    break; 

                if (Random.Range(0, 100) < 50) {
                    foreach (KeyValuePair<string, float> entry in specialDrops) {
                        if (random <= entry.Value) {
                            chestSlots[i].Item = Resources.Load("Items/" + entry.Key) as Item;
                            chestSlots[i].Amount = 1;
                            itemsAdded += 1;
                            break; 
                        }
                    }
                }
            
            }
        }
    }

    private void Update() {
        if (!hasChestBeenFilled) {
            SetItems();
            hasChestBeenFilled = true;
        } 

        spawnPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        realPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
        RaycastHit2D hit = Physics2D.Raycast(realPoint, Vector2.zero);
        float distance = Vector3.Distance(player.position, transform.position);

        hoveringOver = hit.collider != null ? hit.collider.name : "Nothing";
        hoverOverSelf = hit.collider == GetComponent<Collider2D>() && hit.collider != null;

        if (hoverOverSelf && Input.GetMouseButtonDown(1) && !c.IsAnyChestOpen() && !c.isCraftingMenuActive && distance < 5f) {
            OpenChest();
            c.ToggleCraftingMenu();
            isChestOpen = true;
        } else if (Input.GetKeyDown(KeyCode.C) && c.IsAnyChestOpen()) {
            CloseChest();
            c.ToggleCraftingMenu();
            isChestOpen = false;
        }
    }

    void OpenChest() {
        foreach(GameObject g in chestObjects) {
            StartCoroutine(LerpMenu(Vector3.zero, Vector3.one * 0.75f, g));
        }
    }

    void CloseChest() {
        foreach (GameObject g in chestObjects) {
            StartCoroutine(LerpMenu(Vector3.one * 0.75f, Vector3.zero, g));
        }
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        }
    } 

    public void Die() {
        GenerateDrops();
        Destroy(gameObject);
    }

    void GenerateDrops() {
        foreach (ItemSlot itemSlot in chestSlots) {
            if (itemSlot.Item != null) {
                Drop(itemSlot.Item.ItemName, itemSlot.Amount);
            }
        }
        
        Drop("Tree Trunk", 2); Drop("Stripped Log", 2);
    }

    void Drop(string name, int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject drop = Resources.Load("Physical Items/" + name) as GameObject;
            GameObject dropInstance = Instantiate(drop, transform.position, transform.rotation);
            dropInstance.name = string.Format("{0}x{1}", name, amount.ToString());
        }
    }

    public bool IsOverlapping() {
        Collider2D[] above = Physics2D.OverlapCircleAll(top.position, 0.05f);
        Collider2D[] below = Physics2D.OverlapCircleAll(bottom.position, 0.05f);

        for (int a = 0; a < above.Length; a++) {
            if (above[a].gameObject.tag == "Ground") {
                isTGA = true;
                break;
            }
        }

        for (int b = 0; b < below.Length; b++) {
            if (below[b].gameObject.tag == "Ground") {
                isTGB = true;
                break;
            }
        }

        if (isTGA || !isTGB) {
            return true;
        } else {
            return false;
        }
    }

    IEnumerator LerpMenu(Vector3 startScale, Vector3 finalScale, GameObject thing) {
        float elapsedTime = 0f;
        float waitTime = 0.05f;

        while (elapsedTime < waitTime) {
            thing.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.transform.localScale = finalScale;
        yield return null;
    }

}
