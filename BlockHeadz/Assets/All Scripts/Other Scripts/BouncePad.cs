using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour {
    
    [SerializeField] float power = 1750f;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.GetComponent<Rigidbody2D>() != null) {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();

            Vector3 surfaceNormal = other.GetContact(0).normal * -1f;
            rb.AddForce(surfaceNormal * power);
        } 
    }

}
