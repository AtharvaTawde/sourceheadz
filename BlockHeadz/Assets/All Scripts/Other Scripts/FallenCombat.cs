using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenCombat : MonoBehaviour {
    public int currentHealth;
    public GameObject blood;
    public LayerMask player;
    public Transform attackPoint;
    public float attackRange = 1f;

    private int maxHealth = 1000;
    private CameraShake cameraShake;
    private Animator animator;

    private string[] dropNames = {"Eyeball", "Vest", "Stone", "Wood Shard", "Clay Slab", "Baton", "Clay Idol"};
    private int[] dropRates = {25, 75, 100, 100, 12, 6, 3};
    
    void Start() {
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        animator.SetTrigger("Hurt");
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public IEnumerator Hit(int damage) {
        animator.SetTrigger("Attack");
        StartCoroutine(cameraShake.Shake(.15f, .05f));
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, player);
        yield return new WaitForSeconds(.25f);
        foreach (Collider2D player in hitPlayer) {
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 2500);
            player.GetComponent<PlayerCombat>().TakeDamage(damage);
        }            
        yield return new WaitForSeconds(.25f);
    }

    public void Die() {
        StartCoroutine(Blood());
        PlayerPrefsHelper.IncrementInt("Fallens Killed");
        GenerateDrops();
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(blood, transform.position, transform.rotation) as GameObject;
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

        Drop("Baton");
    }

    void Drop(string name) {
        GameObject drop = Resources.Load(name) as GameObject;
        Instantiate(drop, transform.position, transform.rotation);
    }
}
