using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {
    
    private AudioClip submergeSound, emergeSound;

    private void Start() {
        submergeSound = Resources.Load("submerge") as AudioClip;
        emergeSound = Resources.Load("emerge") as AudioClip;
    }

    bool isInteractableWithWater(Collider2D o) {
        if (o.gameObject.GetComponent<Rigidbody2D>() != null && !o.gameObject.tag.Contains("Player")) {
            return true;
        } else {    
            return false;
        }
    }

    #region Physics Interations
    private void OnTriggerEnter2D(Collider2D other) {
        if (isInteractableWithWater(other)) {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playPoint = other.transform.position;
            AudioSource.PlayClipAtPoint(submergeSound, playPoint);
            rb.gravityScale = -2f;
        }  
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (isInteractableWithWater(other)) {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            rb.gravityScale = -2f;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (isInteractableWithWater(other)) {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playPoint = other.transform.position;
            AudioSource.PlayClipAtPoint(emergeSound, playPoint);
            rb.gravityScale = 1f;
        }     
    }
    #endregion

}
