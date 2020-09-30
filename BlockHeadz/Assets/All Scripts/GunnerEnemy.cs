using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerEnemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask pLayer;
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

    public void Fire(bool fire) {
        animator.SetBool("Fire", fire);
    }

    public void Die() {
        Transform bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as Transform;
        Destroy(gameObject);
        Destroy(bloodInstance, 10f);
    }
}
