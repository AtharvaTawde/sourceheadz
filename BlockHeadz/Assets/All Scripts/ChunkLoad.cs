using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChunkLoad : MonoBehaviour {

    public GameObject[] chunks = new GameObject[4];
    public float[] minPointX, maxPointX = new float[4];
    public float playerX; 

    void Start() {
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
    }
}