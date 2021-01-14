using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGeneration : MonoBehaviour {
    
    [SerializeField] GameObject rock, gunflower, chest, titanium;
    [SerializeField] Transform helper;
    [SerializeField] GameObject[] rooms;
    [SerializeField] GameObject[] catacombs;
    [SerializeField] int randomRoom;
    [SerializeField] int randomCatacomb;

    //NOTE: For generated objects, put the IsOverlapping bool in the class and also the checkDestroy bool in the Start function of that class, 
    //then put the script above the default time in the script execution order.
    //Make sure StructureGeneration (this class) is above all other classes related to Generated Objects

    private void OnValidate() {
        for (int i = 0; i < 12; i++) {
            rooms[i] = GameObject.Find("Map/Chunk 8/Dungeons/Red Crystal Room (" + i + ")");
        }

        for (int i = 0; i < 13; i++) {
            catacombs[i] = GameObject.Find("Catacombs/Green Crystal Podium (" + i + ")");
        }
    }

    private void Start() {
        //                                                                            (Visual Only)           (Higher is lower)
        //                Object     Start Spawning At X     Stop Spawning At X      Y Offset    Minimum Frequency   Maximum Frequency   Y Values To Spawn At
        GenerateEntityOnGround(rock,         -15,                    840,              0.5f,       5f,                 15f,                 new int[] {-50, -25, 0, 25, 50, 75, 100});        
        GenerateEntityOnGround(gunflower,    35,                     840,              1.0f,       2f,                 4f,                  new int[] {75, -50, -100});    
        GenerateEntityOnGround(chest,        100,                    840,              0.5f,       10f,                 25f,                new int[] {-25, -37, -50, -62, -75, -87, -100, -112, -125, -137, -150});    
        GenerateEntityOnCeiling(titanium,    50,                     840,              0.5f,       10f,                 15f,                new int[] {-50, -62, -75, -87, -100, -112, -125, -137, -150});    
    
        for (int i = 0; i < 12; i++) {
            rooms[i] = GameObject.Find("Map/Chunk 8/Dungeons/Red Crystal Room (" + i + ")") ;
        }

        for (int i = 0; i < 13; i++) {
            catacombs[i] = GameObject.Find("Catacombs/Green Crystal Podium (" + i + ")");
        }

        GenerateRedCrystal();
        GenerateGreenCrystal();
    }                   

    #region Generate Things
    void GenerateEntityOnGround(GameObject structure, int startX, int stopX, float yOffset, float freqMin, float freqMax, int[] yLevels) {
        float r;
        foreach (int yValue in yLevels) {
            r = Random.Range(freqMin, freqMax);
            for (float i = startX; i < stopX; i += r) {
                helper.position = new Vector3(Mathf.RoundToInt(i) + 0.5f, yValue, 0);
                RaycastHit2D spawnPoint = Physics2D.Raycast(helper.position, Vector2.down);

                if (spawnPoint.collider && (spawnPoint.collider.tag == "Ground" || spawnPoint.collider.tag == "Block")) {
                    helper.position = new Vector3(helper.position.x, spawnPoint.point.y + yOffset, 0f);
                    Instantiate(structure, helper.position, helper.rotation);
                }
                r = Random.Range(freqMin, freqMax);
            }
        }
    }

    void GenerateEntityOnCeiling(GameObject structure, int startX, int stopX, float yOffset, float freqMin, float freqMax, int[] yLevels) {
        float r;
        foreach (int yValue in yLevels) {
            r = Random.Range(freqMin, freqMax);
            for (float i = startX; i < stopX; i += r) {
                helper.position = new Vector3(Mathf.RoundToInt(i) + 0.5f, yValue, 0);
                RaycastHit2D spawnPoint = Physics2D.Raycast(helper.position, Vector2.up);

                if (spawnPoint.collider && (spawnPoint.collider.tag == "Ground" || spawnPoint.collider.tag == "Block")) {
                    helper.position = new Vector3(helper.position.x, spawnPoint.point.y - yOffset, 0f);
                    Instantiate(structure, helper.position, helper.rotation);
                }
                r = Random.Range(freqMin, freqMax);
            }
        }
    }
    
    void GenerateRedCrystal() {
        randomRoom = Random.Range(0, rooms.Length - 1);
        for (int i = 0; i < rooms.Length; i++) {
            if (randomRoom == i) {
                rooms[i].SetActive(true);
            } else {
                rooms[i].SetActive(false);
            }
        }
    }

    void GenerateGreenCrystal() {
        randomCatacomb = Random.Range(0, catacombs.Length - 1);
        for (int i = 0; i < catacombs.Length; i++) {
            if (randomCatacomb == i) {
                catacombs[i].SetActive(true);
            } else {
                catacombs[i].SetActive(false);
            }
        }
    }

    #endregion

}
