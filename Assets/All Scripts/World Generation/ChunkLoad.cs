using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChunkLoad : MonoBehaviour {

    public GameObject[] chunks = new GameObject[8];
    public float[] minPointX = new float[8];
    public float[] maxPointX = new float[8];
    public float playerX; 
    public List<GameObject> entities = new List<GameObject>();
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> blocks = new List<GameObject>();
    [SerializeField] float renderDistance;
    
    void OnValidate() {
        entities.Clear();
        items.Clear();
        blocks.Clear();
        renderDistance = GameObject.Find("Camera Container/Main Camera").GetComponent<Camera>().orthographicSize * 2.5f;
        
        for (int i = 0; i < chunks.Length; i++) {
            chunks[i] = GameObject.Find("Map/Chunk " + (i + 1).ToString());
            minPointX[i] = GameObject.Find("Map/ChunkBegin" + (i + 1).ToString()).transform.position.x;
            maxPointX[i] = GameObject.Find("Map/ChunkEnd" + (i + 1).ToString()).transform.position.x;
        }      
    }

    void Start() {
        renderDistance = GameObject.Find("Camera Container/Main Camera").GetComponent<Camera>().orthographicSize * 2.5f;
        
        foreach(GameObject entity in GameObject.FindGameObjectsWithTag("Enemy")) {
            entities.Add(entity);
        }

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Item")) {
            items.Add(item);
        }

        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block")) {
            blocks.Add(block);
        }
        
        for (int i = 0; i < chunks.Length; i++) {
            chunks[i] = GameObject.Find("Map/Chunk " + (i + 1).ToString());
            minPointX[i] = GameObject.Find("Map/ChunkBegin" + (i + 1).ToString()).transform.position.x;
            maxPointX[i] = GameObject.Find("Map/ChunkEnd" + (i + 1).ToString()).transform.position.x;
        }     
    }

    void Update() {
        renderDistance = GameObject.Find("Camera Container/Main Camera").GetComponent<Camera>().orthographicSize * 5f;
        playerX = transform.position.x;
        for (int i = 0; i < chunks.Length; i++) {
            if (playerX < maxPointX[i] && playerX > minPointX[i]) {
                chunks[i].SetActive(true);
            } else {
                chunks[i].SetActive(false);
            }
        }

        EntityLoading();
        BlockLoading();
        ItemLoading();
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

        int i = 0;
        while (i < entities.Count) {
            if (entities[i] == null) {
                entities.Remove(entities[i]);
            }
            i++;
        }
    }

    void BlockLoading() {
        foreach (GameObject block in blocks) {
            if (block != null) {
                float distance = Vector3.Distance(transform.position, block.transform.position);

                if (distance < renderDistance && !block.activeSelf) {
                    block.SetActive(true);
                } else if (distance > renderDistance && block.activeSelf) {
                    block.SetActive(false);
                }
            }
        }

        int i = 0;
        while (i < blocks.Count) {
            if (blocks[i] == null) {
                blocks.Remove(blocks[i]);
            }
            i++;
        }
    }

    void ItemLoading() {
        foreach (GameObject item in items) {
            if (item != null) {
                float distance = Vector3.Distance(transform.position, item.transform.position);

                if (distance < renderDistance && !item.activeSelf) {
                    item.SetActive(true);
                } else if (distance > renderDistance && item.activeSelf) {
                    item.SetActive(false);
                }
            }
        }

        int i = 0;
        while (i < items.Count) {
            if (items[i] == null) {
                items.Remove(items[i]);
            }
            i++;
        }
    }

}