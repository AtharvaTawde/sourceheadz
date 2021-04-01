using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Gunflower : MonoBehaviour {
    
    [SerializeField] int currentHealth;
    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    
    bool isTGA;
    bool isTGB;
    bool isCheckComplete;

    private static readonly int maxHealth = 50;

    #region Drops
    string[] dropNames = {"Gunflower Seed", "Gunflower Seed", "Gunflower Seed", "Gunflower Seed", "Gunflower Node", "Gunflower Node", "Gunflower Node", "Gunflower Node", "Gunflower Node"};
    int[] dropRates = {100, 50, 25, 12, 50, 25, 12, 6, 3};
    #endregion

    private void Start() {
        currentHealth = maxHealth;

        if (!isCheckComplete) {
            if (IsOverlapping()) {
                Destroy(gameObject);
            }
            isCheckComplete = true;    
        }
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        HelperFunctions.IncrementInt("Gunflowers Harvested");
        GenerateDrops();
        Destroy(gameObject);
    }

    void GenerateDrops() {
        for (int i = 0; i < dropNames.Length; i++) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= dropRates[i]) {
                Drop(dropNames[i], 1);
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
        Collider2D[] above = Physics2D.OverlapCircleAll(top.position, 0.20f);
        Collider2D[] below = Physics2D.OverlapCircleAll(bottom.position, 0.45f);

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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;

        Gizmos.DrawWireSphere(top.position, 0.20f);
        Gizmos.DrawWireSphere(bottom.position, 0.45f);    
    }

}
