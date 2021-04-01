using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Stay In Radius")]
public class StayInRadiusBehavior : FlockBehavior {
    
    [SerializeField] Vector2 center;

    private float radius = 30;

    public override Vector2 CalculateMove(Fish fish, List<Transform> context, Flock flock) {
        Vector2 centerOffset = center - (Vector2)fish.transform.position;
        float t = centerOffset.magnitude / radius;
        if (t < 0.9f) {
            return Vector2.zero;
        }
        return centerOffset * t * t;
    }

}
