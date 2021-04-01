using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    
    public IEnumerator Shake(float duration, float magnitude) {
        Vector3 resetVector = new Vector3(0f, 0f, -10f);
        float elapsed = 0.0f;
        while (elapsed < duration) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, -10f);
            elapsed += Time.deltaTime;
            yield return null; 
        }

        transform.localPosition = resetVector;
    }
}
