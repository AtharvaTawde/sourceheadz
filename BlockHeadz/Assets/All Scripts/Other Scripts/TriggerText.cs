using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerText : MonoBehaviour {
    
    TextMeshProUGUI textComp;
    GameObject player;
    float distanceNecesito = 7f; 

    private void Start() {
        textComp = GetComponent<TextMeshProUGUI>();
        textComp.color = new Color(1f, 1f, 1f, 0f);
        textComp.enabled = false;
        player = GameObject.Find("Player");
    }

    private void Update() {
        float distance = Vector3.Distance(transform.position, player.transform.position);
    
        if (distance < distanceNecesito) {
            StartCoroutine(TrigText());
        } else if (distance >= distanceNecesito && textComp.enabled) {
            StartCoroutine(UnTrigText());
        }
    }

    IEnumerator TrigText() {
        textComp.enabled = true;
        textComp.color = Vector4.Lerp(textComp.color, new Vector4(1f, 1f, 1f, 1f), Time.deltaTime);
        yield return null;
    }

    IEnumerator UnTrigText() {
        textComp.color = Vector4.Lerp(textComp.color, new Vector4(1f, 1f, 1f, 0f), Time.deltaTime);
        yield return null;
    }
}
