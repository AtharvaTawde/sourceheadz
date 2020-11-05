using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChunkLoad : MonoBehaviour {

    public GameObject[] chunks = new GameObject[7];
    public float[] minPointX = new float[7];
    public float[] maxPointX = new float[7];
    public float playerX; 
    [SerializeField] GameObject[] entities;
    float renderDistance = 12f;
    
    void OnValidate() {
        entities = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < chunks.Length; i++) {
            chunks[i] = GameObject.Find("Map/Chunk " + (i + 1).ToString());
            minPointX[i] = GameObject.Find("Map/ChunkBegin" + (i + 1).ToString()).transform.position.x;
            maxPointX[i] = GameObject.Find("Map/ChunkEnd" + (i + 1).ToString()).transform.position.x;
        }        
    }

    void Start() {
        entities = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < chunks.Length; i++) {
            chunks[i] = GameObject.Find("Map/Chunk " + (i + 1).ToString());
            minPointX[i] = GameObject.Find("Map/ChunkBegin" + (i + 1).ToString()).transform.position.x;
            maxPointX[i] = GameObject.Find("Map/ChunkEnd" + (i + 1).ToString()).transform.position.x;
        }        
    }

    void Update() {
        playerX = transform.position.x;
        for (int i = 0; i < chunks.Length; i++) {
            if (playerX < maxPointX[i] && playerX > minPointX[i]) {
                chunks[i].SetActive(true);
            } else {
                chunks[i].SetActive(false);
            }
        }

        EntityLoading();
    }

    void EntityLoading() {
        foreach (GameObject entity in entities) {
            if (entity != null) {
                float distance = Vector3.Distance(transform.position, entity.transform.position);

                if (distance < renderDistance) {
                    entity.SetActive(true);
                } else if (distance > renderDistance) {
                    entity.SetActive(false);
                }
            }
        }
    }

}