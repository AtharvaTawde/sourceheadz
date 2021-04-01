using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {
    
    [SerializeField] List<Fish> fish = new List<Fish>();
    private const float agentDensity = 0.08f; 
    private int startingCount = 100;
    private float driveFactor = 10f;
    private float maxSpeed = 5f;
    private float neighborRadius = 1.5f;
    private float avoidanceRadiusMultiplier = 0.5f; 
    private float squareMaxSpeed;
    private float squareNeighborRadius;
    private float squareAvoidanceRadius; 
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    [SerializeField] GameObject fishPrefab; 
    [SerializeField] FlockBehavior behavior;
    [SerializeField] ChunkLoad chunkLoad;

    void Start() {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++) {
            GameObject newFish = Instantiate(fishPrefab, Random.insideUnitCircle * startingCount * agentDensity, Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)), transform);
            newFish.name = string.Format("Fish {0}", i);
            newFish.transform.position += transform.position;
            fish.Add(newFish.GetComponent<Fish>());
            chunkLoad.entities.Add(newFish);
        }
    }

    void Update() {
        foreach(Fish f in fish) {
            if (f != null) {
                List<Transform> cont = GetNearbyObjects(f);
                Vector2 move = behavior.CalculateMove(f, cont, this);
                move *= driveFactor;
                if (move.sqrMagnitude > squareMaxSpeed) {
                    move = move.normalized * maxSpeed;
                }
                f.Move(move);
            }
        }

        int fishIndex = 0;
        while (fishIndex < fish.Count) {
            if (fish[fishIndex] == null) {
                fish.RemoveAt(fishIndex);
            }
            fishIndex++;
        }
    }

    List<Transform> GetNearbyObjects(Fish fish) {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(fish.transform.position, neighborRadius);
        foreach (Collider2D c in contextColliders) {
            if (c != fish.AgentCollider && c.tag != "Water" && c.tag != "Ground") {
                context.Add(c.transform);
            }
        }
        return context; 
    }

}
