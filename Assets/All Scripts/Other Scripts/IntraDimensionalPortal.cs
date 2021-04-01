using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntraDimensionalPortal : MonoBehaviour {
    
    [SerializeField] Transform aerInitial;
    [SerializeField] Transform aerFinal; 

    private void OnTriggerEnter2D(Collider2D other) {
        
        string tag = other.gameObject.tag;
        string name = other.gameObject.name;
        string portal;
        bool isEntity;

        #region String and Bool Builders
        if (tag == "Player" || tag == "Enemy" || name.Contains("Revenant")) {
            isEntity = true;
        } else {
            isEntity = false;
        }

        if (gameObject.name.Contains("Source")) {
            portal = "Source";
        } else if (gameObject.name.Contains("Destination")) {
            portal = "Destination";
        } else {
            portal = "Nothing";
        }

        //Debug.Log("Portal Type: " + portal + " IsEntity: " + isEntity);
        #endregion

        if (isEntity && portal == "Source") {
            Vector3 transportVector = new Vector3(aerFinal.position.x + 5f, aerFinal.position.y, aerFinal.position.z);
            other.transform.position = transportVector;
        } else if (isEntity && portal == "Destination") {
            Vector3 transportVector = new Vector3(aerInitial.position.x - 5f, aerInitial.position.y, aerInitial.position.z);
            other.transform.position = transportVector;
        }
    }

}
