using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenameObject : MonoBehaviour {
    
    [SerializeField] GameObject nyertsParent, goblinsParent, archersParent, fallensParent, revenantsParent, firetrapsParent;
    [SerializeField] SpriteRenderer[] nyerts, goblins, archers, fallens, revenants, firetraps;
 
    private void OnValidate() {
        nyerts = nyertsParent.GetComponentsInChildren<SpriteRenderer>();
        goblins = goblinsParent.GetComponentsInChildren<SpriteRenderer>();
        archers = archersParent.GetComponentsInChildren<SpriteRenderer>();
        fallens = fallensParent.GetComponentsInChildren<SpriteRenderer>();
        revenants = revenantsParent.GetComponentsInChildren<SpriteRenderer>();
        firetraps = firetrapsParent.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < nyerts.Length; i++) {
            nyerts[i].gameObject.name = "Nyert" + i;
        }

        for (int i = 0; i < goblins.Length; i++) {
            goblins[i].gameObject.name = "Goblin" + i;
        }

        for (int i = 0; i < archers.Length; i++) {
            archers[i].gameObject.name = "Archer" + i;
        }

        for (int i = 0; i < fallens.Length; i++) {
            fallens[i].gameObject.name = "Fallen" + i;
        }

        for (int i = 0; i < revenants.Length; i++) {
            revenants[i].gameObject.name = "Revenant" + i;
        }

        for (int i = 0; i < firetraps.Length; i++) {
            firetraps[i].gameObject.name = "Firetrap" + i;
        }
    } 

}
