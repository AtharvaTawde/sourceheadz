using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunflowerSeed : MonoBehaviour {

    [SerializeField] Sprite[] stages;    
    [SerializeField] GameObject gunflower;
    [SerializeField] int currentStage = 0;
    [SerializeField] float timeToGrow;
   
    private void Start() {
        timeToGrow = Random.Range(30f, 60f);
    }

    private void Update() {
        if (timeToGrow > 0) {
            timeToGrow -= Time.deltaTime;
        } else if (timeToGrow <= 0 && currentStage != stages.Length - 1) {
            currentStage++;
            timeToGrow = Random.Range(30f, 60f);
        }

        GetComponent<SpriteRenderer>().sprite = stages[currentStage];

        if (currentStage == stages.Length - 1) {
            Instantiate(gunflower, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

}
