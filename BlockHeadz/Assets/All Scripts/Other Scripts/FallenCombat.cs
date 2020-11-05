using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenCombat : MonoBehaviour {
    public int maxHealth;
    public int currentHealth;
    public Transform Explosion;
    public LayerMask player;
    public Transform attackPoint;
    public float attackRange = 1f;

    private CameraShake cameraShake;
    private Animator animator;
    
    void Start() {
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
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
        Transform bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as Transform;
        Destroy(gameObject);
        Destroy(bloodInstance, 10f);
    }
}
