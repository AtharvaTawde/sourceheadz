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
    private bool hurt;

    private string[] dropNames = {"Eyeball", "Stone", "Wood Shard", "Clay Slab", "Baton", "Clay Idol"};
    private int[] dropRates = {25, 100, 100, 12, 6, 3};
    
    void Start() {
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Update() {
        if (hurt)
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
    }

    public void TakeDamage(int damage) {
        hurt = true;
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
        HelperFunctions.IncrementInt("Fallens Killed");
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
                Drop(dropNames[i], 1);
            }
        }

        Drop("Baton", 1);
    }

    void Drop(string name, int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject drop = Resources.Load("Physical Items/" + name) as GameObject;
            GameObject dropInstance = Instantiate(drop, transform.position, transform.rotation);
            dropInstance.name = string.Format("{0}x{1}", name, amount.ToString());
        }
    }
}
