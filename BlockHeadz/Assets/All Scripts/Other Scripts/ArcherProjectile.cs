using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherProjectile : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D other) {
        
        GameObject shot = other.gameObject;
        
        if (shot.tag == "Player") {
        
            shot.GetComponent<PlayerCombat>().TakeDamage(45);
        
        } else if (shot.tag == "Enemy") {
            
            if (shot.name.Contains("Nyert")) {
                shot.GetComponent<Enemy>().TakeDamage(45);
            } else if (shot.name.Contains("Goblin") || shot.name.Contains("Zombi")) {
                shot.GetComponent<GoblinEnemy>().TakeDamage(45);
            } else if (shot.name.Contains("Archer")) {
                shot.GetComponent<ArcherCombat>().TakeDamage(45);
            }
        
        }
        
        if (other.gameObject.tag != "Untagged")
            Destroy(gameObject);
    
    }

}
