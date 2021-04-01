using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveSpikes : MonoBehaviour {
    
    public float ph;
    [SerializeField] Stratum stratum; 
    private bool beginDetachment = true; 

    void Update() {
        ph = (float)stratum.currentHealth / (float)stratum.maxHealth; 
        if (ph <= 0.25f && beginDetachment) {
            StartCoroutine(DetachSpikes());
            beginDetachment = false;
        }
    }

    IEnumerator DetachSpikes() {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject spike = transform.GetChild(i).gameObject; 
            if (spike.name == string.Format("Cave Spike{0}", i)) {
                spike.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                Destroy(spike, 60f);
            }
            yield return new WaitForSeconds(2f);
        }
    }

}
