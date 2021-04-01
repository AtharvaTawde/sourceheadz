using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnItem : MonoBehaviour {
    
    GameObject player;

    #region Space-Based Despawn Algorithm
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name.Contains("World Boundary") || other.gameObject.name == "Death") {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.name.Contains("World Boundary") || other.gameObject.name == "Death") {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Despawn After 5 Minute
    [SerializeField] float actualTime;
    private static float despawnConstant = 1200f; // 20 minutes

    private void Start() {
        player = GameObject.Find("Player");
        player.GetComponent<ChunkLoad>().items.Add(gameObject);
        actualTime = despawnConstant;
    }

    private void Update() {
        if (!gameObject.name.Contains("Crystal") && !gameObject.name.Contains("Boss")) {
            if (actualTime > 0) {
                actualTime -= Time.deltaTime;
            } else {
                Destroy(gameObject);
            }
        }
    }
    #endregion

}
