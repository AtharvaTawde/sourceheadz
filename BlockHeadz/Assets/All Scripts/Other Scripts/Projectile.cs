using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    float posX;

    private void Update() {
        if (gameObject.activeInHierarchy) {
            posX = transform.position.x;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        GameObject shot = other.gameObject;
        Rigidbody2D rb = shot.GetComponent<Rigidbody2D>();
        
        if (gameObject.name.Contains("Archer")) {
            if (shot.tag == "Player") {
                shot.GetComponent<PlayerCombat>().TakeDamage(22);
            } else if (shot.tag == "Enemy") {
                if (shot.name.Contains("Nyert")) {
                    shot.GetComponent<Enemy>().TakeDamage(22);
                } else if (shot.name.Contains("Goblin") || shot.name.Contains("Zombi")) {
                    shot.GetComponent<GoblinEnemy>().TakeDamage(22);
                } else if (shot.name.Contains("Archer")) {
                    shot.GetComponent<ArcherCombat>().TakeDamage(22);
                } else if (shot.name.Contains("Revenant")) {
                    shot.GetComponent<RevenantPatrol>().TakeDamage(22);
                }
            }
        } else if (gameObject.name.Contains("Revenant")) {
            if (shot.tag == "Player") {
                shot.GetComponent<PlayerCombat>().TakeDamage(15);
            } else if (shot.tag == "Enemy") {
                if (shot.name.Contains("Nyert")) {
                    shot.GetComponent<Enemy>().TakeDamage(15);
                } else if (shot.name.Contains("Goblin") || shot.name.Contains("Zombi")) {
                    shot.GetComponent<GoblinEnemy>().TakeDamage(15);
                } else if (shot.name.Contains("Archer")) {
                    shot.GetComponent<ArcherCombat>().TakeDamage(15);
                } else if (shot.name.Contains("Revenant")) {
                    shot.GetComponent<RevenantPatrol>().TakeDamage(15);
                }
            }

            float power = 1500f;

            if (shot.transform.position.x < posX && rb != null) {
                shot.GetComponent<Rigidbody2D>().AddForce(Vector2.right * power);
            } else {
                shot.GetComponent<Rigidbody2D>().AddForce(Vector2.left * power);
            }
        }
        
        Destroy(gameObject);
    }

}
