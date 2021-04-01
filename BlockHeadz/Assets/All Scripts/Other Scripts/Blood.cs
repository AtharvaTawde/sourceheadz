using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour {
    
    [SerializeField] float selfDestructTimer;

    void Update() {
        Destroy(gameObject, selfDestructTimer);
    }
    
}
