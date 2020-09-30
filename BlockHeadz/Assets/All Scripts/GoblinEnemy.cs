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
    public LayerMask pLayer;
    public Transform Explosion;

    void Start() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            Die();
        }
    }

    public IEnumerator Hit(int damage) {
        animator.SetBool("Hit", true);
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, pLayer);
        yield return new WaitForSeconds(.5f);
        foreach (Collider2D player in hitPlayer) {
            player.GetComponent<PlayerCombat>().TakeDamage(damage);
        }
        yield return new WaitForSeconds(.25f);
        animator.SetBool("Hit", false);
    }

    public void Die() {
        Transform bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as Transform;
        
        if (gameObject.name.Contains("Goblin")) {
            PlayerPrefsHelper.IncrementInt("Goblins Killed");
        } else if (gameObject.name.Contains("Zombi")) {
            PlayerPrefsHelper.IncrementInt("Zombies Killed");
        }

        Destroy(gameObject);
        Destroy(bloodInstance, 10f);
    }
}

public static class PlayerPrefsHelper {
    
    public static void IncrementInt(string key) {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
    }

}
