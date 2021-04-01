using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Steer Cohesion")]
public class SteerCohesionBehavior : FlockBehavior {

    private Vector2 currentVelocity;
    private float fishSmoothTime = 0.5f;

    public override Vector2 CalculateMove(Fish fish, List<Transform> context, Flock flock) {
        // if no neighbors, return no adjustment (no need to turn)
        if (context.Count == 0)
            return Vector2.zero;

        // add all points together and average
        Vector2 cohesionMove = Vector2.zero;
        foreach (Transform item in context) {
            cohesionMove += (Vector2)item.position;
        }
        cohesionMove /= context.Count;

        //create offset from agent position
        cohesionMove -= (Vector2)fish.transform.position;
        cohesionMove = Vector2.SmoothDamp(-fish.transform.right, cohesionMove, ref currentVelocity, fishSmoothTime);
        return cohesionMove;
    }

}
