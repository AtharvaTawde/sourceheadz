using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunflowerChild : MonoBehaviour {
    
    public string isTouchingGround = "Unknown";

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ground") && gameObject.name == "Top") {
            isTouchingGround = "Yes";
        } else {
            isTouchingGround = "No";
        }

        if (other.gameObject.CompareTag("Ground") && gameObject.name == "Bottom") {
            isTouchingGround = "Yes";
        } else {
            isTouchingGround = "No";
        }
    }

}
