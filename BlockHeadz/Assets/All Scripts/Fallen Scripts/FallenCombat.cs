using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenCombat : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Transform Explosion;
    public LayerMask player;
    public Transform attackPoint;
    public float attackRange = 1f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Hit(int damage) {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, player);
        foreach (Collider2D player in hitPlayer) {
            player.GetComponent<PlayerCombat>().TakeDamage(damage);
            player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Random.Range(1000, 1500)));
        }
    }

    public void Die() {
        if (gameObject.tag == "Enemy") {
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
