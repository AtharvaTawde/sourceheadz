using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public Transform Explosion;
    // Start is called before the first frame update
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

    public void Die() {
        if (gameObject.tag == "Enemy") {
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
