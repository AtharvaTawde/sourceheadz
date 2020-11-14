using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunflower : MonoBehaviour {
    
    [SerializeField] int currentHealth;
    private static readonly int maxHealth = 50;

    #region Drops
    string[] dropNames = {"Gunflower Seed", "Gunflower Seed", "Gunflower Seed", "Gunflower Seed", "Gunflower Node", "Gunflower Node", "Gunflower Node", "Gunflower Node", "Gunflower Node"};
    int[] dropRates = {100, 50, 25, 12, 50, 25, 12, 6, 3};
    #endregion

    private void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        GenerateDrops();
        Destroy(gameObject);
    }

    void GenerateDrops() {
        for (int i = 0; i < dropNames.Length; i++) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= dropRates[i]) {
                Drop(dropNames[i]);
            }
        }
    }

    void Drop(string name) {
        GameObject drop = Resources.Load(name) as GameObject;
        Instantiate(drop, transform.position, transform.rotation);        
    }

}
