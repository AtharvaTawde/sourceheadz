using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public void SelfDestruct() {
        if (gameObject.tag == "Health") {
            Destroy(gameObject);
        }
    }

    public void DestroyKey() {
        if (gameObject.tag == "Key") {
            Destroy(gameObject);
        }
    }
}
