using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour {
    [SerializeField] Sprite[] stages;    
    [SerializeField] GameObject babyChicken;
    [SerializeField] GameObject eggExplosionParticles;
    [SerializeField] int currentStage = 0;
    [SerializeField] float timeToGrow;
    
    private void Start() {
        timeToGrow = Random.Range(60, 120);
    }

    private void Update() {
        if (timeToGrow > 0) {
            timeToGrow -= Time.deltaTime;
        } else if (timeToGrow <= 0 && currentStage != stages.Length - 1) {
            currentStage++;
            timeToGrow = Random.Range(60, 120);
        }

        GetComponent<SpriteRenderer>().sprite = stages[currentStage];

        if (currentStage == stages.Length - 1) {
            Instantiate(eggExplosionParticles, transform.position, transform.rotation);
            Instantiate(babyChicken, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
