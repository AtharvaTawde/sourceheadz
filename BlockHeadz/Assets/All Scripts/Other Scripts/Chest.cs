using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {
   
    [SerializeField] int currentHealth;
    [SerializeField] Sprite specialChestSprite;
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    
    private bool isTGA;
    private bool isTGB;
    private bool isCheckComplete;
    private int maxHealth = 150;

    private Dictionary<string, int> regularDrops = new Dictionary<string, int> {
        {"Diamond",         250},
        {"Titanium Bar",    350},
        {"Gold Scrap",      450},
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
        {"Varium Boots",                    10},
        {"Diamond Chestplate",              20},
        {"Diamond Sword",                   30},
        {"Diamond Leggings",                50},
        {"Diamond Boots",                   100},
        {"Titanium Chestplate",             125},
        {"Titanium Sword",                  200},
        {"Titanium Leggings",               250},
        {"Varium",                          250},
        {"Titanium Boots",                  500},
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
        if (!gameObject.name.Contains("Special")) {

            List<string> itemsDropped = new List<string>();
            foreach (KeyValuePair<string, int> entry in regularDrops) {
                int random = Mathf.RoundToInt(Random.Range(0, 1000));
                if (itemsDropped.Count == 3) {
                    break;
                }

                if (random <= entry.Value) {
                    Drop(entry.Key, 1);
                    itemsDropped.Add(entry.Key);
                }
            }
            Drop("Bread Scrap", Random.Range(1, 2));
        
        } else {

            List<string> itemsDropped = new List<string>();
            foreach (KeyValuePair<string, float> entry in specialDrops) {
                int random = Mathf.RoundToInt(Random.Range(0, 1000));

                if (itemsDropped.Count == 2) {
                    break;
                }

                if (random <= entry.Value) {
                    Drop(entry.Key, 1);
                    itemsDropped.Add(entry.Key);
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

}
