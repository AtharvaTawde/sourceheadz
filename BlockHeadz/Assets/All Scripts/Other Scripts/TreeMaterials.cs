using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMaterials : MonoBehaviour {
    
    [SerializeField] string id;
    [SerializeField] int currentHealth;

    private int maxHealth;
    private string[] woodDrops = {"Tree Trunk"};
    private int[] woodDropRates = {100};
    private string[] leafDrops = {"Wood Shard", "Wood Shard", "Wood Shard", "Wood Shard", "Wood Shard", "Wood Shard", "Wood Shard", "Wood Shard"};
    private int[] leafDropRates = {100, 50, 25, 12, 6, 3, 2, 1};
    private string[] appleDrops = {"Apple"};
    private int[] appleDropRates = {100, 100};
    private string[] logDrops = {"Stripped Log"};
    private int[] logDropRates = {100};
    private string[] stoneBlockDrops = {"Stone Block"};
    private int[] stoneBlockDropRates = {100};
    private string[] ovenDrops = {"Oven"};
    private int[] ovenDropRates = {100};

    private void Start() {
        id = gameObject.name;

        if (id.Contains("Trunk")) {
            maxHealth = 100;
        } else if (id.Contains("Leaves")) {
            maxHealth = 50;
        } else if (id.Contains("Apples")) {
            maxHealth = 75;
        } else if (id.Contains("Log")) {
            maxHealth = 100;
        } else if (id.Contains("Stone Block")) {
            maxHealth = 150;
        } else if (id.Contains("Oven")) {
            maxHealth = 350;
        }

        currentHealth = maxHealth;
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
        if (id.Contains("Trunk")) {
            for (int i = 0; i < woodDrops.Length; i++) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= woodDropRates[i]) {
                    Drop(woodDrops[i], 1);
                }
            }
        } else if (id.Contains("Leaves")) {
            for (int i = 0; i < leafDrops.Length; i++) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= leafDropRates[i]) {
                    Drop(leafDrops[i], 1);
                }
            }
        } else if (id.Contains("Apples")) {
            for (int i = 0; i < appleDrops.Length; i++) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= appleDropRates[i]) {
                    Drop(appleDrops[i], 1);
                }
            }
        } else if (id.Contains("Log")) {
            for (int i = 0; i < logDrops.Length; i++) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= logDropRates[i]) {
                    Drop(logDrops[i], 1);
                }
            }
        } else if (id.Contains("Stone Block")) {
            for (int i = 0; i < stoneBlockDrops.Length; i++) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= stoneBlockDropRates[i]) {
                    Drop(stoneBlockDrops[i], 1);
                }
            }
        } else if (id.Contains("Oven")) {
            for (int i = 0; i < ovenDrops.Length; i++) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= ovenDropRates[i]) {
                    Drop(ovenDrops[i], 1);
                }
            }

            foreach (string item in GetComponent<Oven>().foodInOven) {
                Drop(item, 1);
            }

            foreach (string fuelItem in GetComponent<Oven>().fuelInOven) {
                Drop(fuelItem, 1);
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
