using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRenamer : MonoBehaviour {
    
    [SerializeField] GameObject crack;

    private void OnValidate() {
        Enumerator();
    }

    void Enumerator() {
        int childID = 0;
        foreach (Transform child in transform) {
            child.name = child.name.RemoveIntegers();
            child.name += childID;
            childID += 1;
        }
    }

    void AddChildObjects() {
        crack = Resources.Load("Crack") as GameObject;
        foreach (Transform child in transform) {
            if (child.childCount == 0) {
                GameObject instance = Instantiate(crack, Vector3.zero, transform.rotation);
                instance.transform.parent = child.transform;
            }
        }
    }

    void RemoveChildObjects() {
        foreach (Transform tree in transform) {
            foreach (Transform child in tree) {
                foreach (Transform obj in child) {
                    DestroyImmediate(obj.gameObject);
                }
            }
        }
    }

}
