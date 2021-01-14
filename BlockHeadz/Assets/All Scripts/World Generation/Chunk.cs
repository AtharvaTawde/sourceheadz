using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    
    [SerializeField] GameObject StrippedLogTile, TreeTrunkTile, StoneTile;     
    
    [HideInInspector]
    public int width = 1;
    [HideInInspector]
    public int heightAddition = 2;
    [HideInInspector]
    public float heightMultiplier = 50;
    [HideInInspector]
    public float smoothness = 75;
    [HideInInspector]
    public float seed = 0;

    private float holeChance = 2;

    void Start() {
        Generate();
    }

    void Generate() {
        for (int i = 0; i < width; i++) {
            int h = Mathf.RoundToInt(Mathf.PerlinNoise(seed, (i + transform.position.x) / smoothness) * heightMultiplier);
            for (int j = 0; j < h; j++) {
                GameObject selectedTile;
                if (j < h - 4) {
                    selectedTile = StoneTile; // The rest of the layers
                } else if (j < h - 1) {
                    selectedTile = TreeTrunkTile; // Next 3 layers
                } else {
                    selectedTile = StrippedLogTile; // Top 1 layer
                }

                GameObject newTile = Instantiate(selectedTile, Vector3.zero, Quaternion.identity);
                newTile.transform.parent = this.gameObject.transform;
                newTile.transform.localPosition = new Vector3(i + 0.5f, j + 0.5f);
            }
        }

    }

}
