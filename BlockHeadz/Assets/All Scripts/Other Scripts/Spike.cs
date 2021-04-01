using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

    [SerializeField] float stratumPercentageHealth;
    
    private void Start() {
        stratumPercentageHealth = transform.parent.GetComponent<CaveSpikes>().ph;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (stratumPercentageHealth <= 0.25f) {
            if (other.gameObject.tag == "Player") {
                if (!other.gameObject.GetComponent<PlayerMovement>().isDead)
                    other.gameObject.GetComponent<PlayerCombat>().TakeDamage(500);
            }    

            if (other.gameObject.tag == "Block") {
                other.gameObject.GetComponent<TakeDamage>().BlockDrop();
            }
        }
    }

}
