using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    GameObject Explosion;

    # region Drops
    string[] dropNames = {"Nyert Meat", "Fabric", "Fabric", "Fabric", "Fabric", "Fabric"};
    int[] dropRates = {75, 100, 50, 25, 12, 6};
    # endregion

    void Start() {
        currentHealth = maxHealth;
        Explosion = Resources.Load("Blood") as GameObject;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        int random = Mathf.RoundToInt(Random.Range(0, 100));
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        PlayerPrefsHelper.IncrementInt("Nyerts Killed");
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
