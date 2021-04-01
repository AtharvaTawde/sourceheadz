using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehavior : FlockBehavior {

    public override Vector2 CalculateMove(Fish fish, List<Transform> context, Flock flock) {
        // if no neighbors, maintain current alignment
        if (context.Count == 0)
            return -fish.transform.right;

        // add all points together and average
        Vector2 alignmentMove = Vector2.zero;
        foreach (Transform item in context) {
            alignmentMove += -(Vector2)item.transform.right;
        }
        alignmentMove /= context.Count;
        return alignmentMove;
    }

}
