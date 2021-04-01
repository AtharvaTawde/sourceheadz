using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Avoidance")]
public class AvoidanceBehavior : FlockBehavior {

    public override Vector2 CalculateMove(Fish fish, List<Transform> context, Flock flock) {
        // if no neighbors, return no adjustment (no need to turn)
        if (context.Count == 0)
            return Vector2.zero;

        // add all points together and average
        Vector2 avoidanceMove = Vector2.zero;
        int nAvoid = 0;
        foreach (Transform item in context) {
            if (Vector2.SqrMagnitude(item.position - fish.transform.position) < flock.SquareAvoidanceRadius) {
                nAvoid++;
                avoidanceMove += (Vector2)(fish.transform.position - item.position);
            }
        }
        if (nAvoid > 0) {
            avoidanceMove /= nAvoid;
        }
        return avoidanceMove;
    }

}
