using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamage : MonoBehaviour {

    [SerializeField] string self;

    private void Start() {
        self = gameObject.name;
    }

    public void ReceiveDamage(int damage) {
        if (self.Contains("Nyert")) {
            GetComponent<Enemy>().TakeDamage(damage);
        } else if (self.Contains("Goblin")) {
            GetComponent<GoblinEnemy>().TakeDamage(damage);
        } else if (self.Contains("Fallen")) {
            GetComponent<FallenCombat>().TakeDamage(damage);
        } else if (self.Contains("Archer")) {
            GetComponent<ArcherCombat>().TakeDamage(damage);
        } else if (self.Contains("Revenant")) {
            GetComponent<RevenantPatrol>().TakeDamage(damage);
        } else if (self.Contains("Gunflower")) {
            GetComponent<Gunflower>().TakeDamage(damage);
        }
    }

}
    