using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

public class NyertCombat : MonoBehaviour {

    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public float burnTime = 0f;
    
    [SerializeField] GameObject onFireGraphic;

    private GameObject Explosion;    
    private bool hurt;
    private float burnCooldown = 0f;

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"Nyert Meat", 75},
        {"Fabric1", 100},
        {"Fabric2", 50},
        {"Fabric3", 25},
        {"Fabric4", 12},
        {"Fabric5", 6}
    };

    private Dictionary<string, int> burnDrops = new Dictionary<string, int> {
        {"Cooked Nyert Meat", 75},
        {"Fabric1", 100},
        {"Fabric2", 50},
        {"Fabric3", 25},
        {"Fabric4", 12},
        {"Fabric5", 6}
    };

    void Start() {
        currentHealth = maxHealth;
        Explosion = Resources.Load("Blood") as GameObject;
    }

    private void Update() {

        Burn();

        if (hurt)
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
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
        HelperFunctions.IncrementInt("Nyerts Killed");
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

