using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask pLayer;
    public Transform Explosion;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Hit(bool hit, int damage) {
        animator.SetBool("Hit", hit);
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, pLayer);
        foreach (Collider2D player in hitPlayer) {
            player.GetComponent<PlayerCombat>().TakeDamage(attackDamage);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(25, 1));
        }

    }

    public void Die() {
        if (gameObject.tag == "Enemy") {
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
