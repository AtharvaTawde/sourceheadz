using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    GameObject Explosion;
    [SerializeField] GameObject nyertMeat;
    [SerializeField] GameObject vest;
    int meatDropPercentChance = 75;
    int vestDropPercentChance = 45;

    void Start() {
        currentHealth = maxHealth;
        nyertMeat = Resources.Load("Nyert Meat") as GameObject;
        vest = Resources.Load("Vest") as GameObject;
        Explosion = Resources.Load("Blood") as GameObject;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        int random = Mathf.RoundToInt(Random.Range(0, 100));
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            StartCoroutine(Blood());

            if (random < meatDropPercentChance) {
                Instantiate(nyertMeat, transform.position, transform.rotation);
            }

            if (random < vestDropPercentChance) {
                Instantiate(vest, transform.position, transform.rotation);
            }

            Die();
        }
    }

    public void Die() {
        PlayerPrefsHelper.IncrementInt("Nyerts Killed");
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }
}
