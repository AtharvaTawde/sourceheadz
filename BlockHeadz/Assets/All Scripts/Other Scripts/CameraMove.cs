using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    float timeItTakesToDoLerping = 100f;

    void Update() {
        if (transform.position == new Vector3(13.9f, -9.5f, -10f)) {
            Vector3 targetPosition = new Vector3(434.8f, -9.5f, -10f);
            StartCoroutine(LerpPosition(targetPosition, timeItTakesToDoLerping)); 
        } else if (transform.position == new Vector3(434.8f, -9.5f, -10f)) {
            Vector3 targetPosition = new Vector3(13.9f, -9.5f, -10f);
            StartCoroutine(LerpPosition(targetPosition, timeItTakesToDoLerping));
        }
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration) {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
