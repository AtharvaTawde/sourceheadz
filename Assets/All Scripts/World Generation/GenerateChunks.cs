using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateChunks : MonoBehaviour {
   public GameObject Chunk;
   int chunkWidth;
   float chunkHeight;
   public int numChunks;
   float seed;

   private void Start() {
       chunkWidth = Chunk.GetComponent<Chunk>().width;
       seed = Random.Range(-100000f, 100000f);
       Generate();
   }

   public void Generate() {
       int lastX = -chunkWidth;
       for (int i = 0; i < numChunks; i++) {
           GameObject NewChunk = Instantiate(Chunk, new Vector3(lastX + chunkWidth, 0f), Quaternion.identity) as GameObject;
           NewChunk.name = string.Format("Chunk{0}", i + 1);
           NewChunk.transform.parent = transform;
           NewChunk.GetComponent<Chunk>().seed = seed;
           lastX += chunkWidth;
       }
   }
}
