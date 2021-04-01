using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popper : MonoBehaviour {
    
    [SerializeField] Transform detonationPoint;
    [SerializeField] AudioClip explodeSound;
    [SerializeField] GameObject explosion;
    
    private float power = 1000f;
    private float radius = 4f;
    private float detonateTime = 3f;
    private float randomPitch;
    private bool detonate;
    private CameraShake cameraShake;
    private AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        explodeSound = Resources.Load("explode") as AudioClip;
    }

    private void Update() {
        randomPitch = Random.Range(.5f, 1f);
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();

        if (detonate) {
            StartCoroutine(cameraShake.Shake(.5f, 1f));
        }

        if (detonateTime > 0) {
            detonateTime -= Time.deltaTime;
        } else if (detonateTime <= 0) {
            StartCoroutine(Explode());
            detonateTime = Mathf.Infinity;
        }
    }

    private IEnumerator Explode() {
        audioSource.PlayOneShot(explodeSound, randomPitch);
        GameObject expInstance = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(expInstance, 0.8f);
        GetComponent<SpriteRenderer>().enabled = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(detonationPoint.position.x, detonationPoint.position.y), radius);
        
        foreach (Collider2D hit in colliders) {
            detonate = true;
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            PlayerCombat player = hit.GetComponent<PlayerCombat>();
            TakeDamage creature = hit.GetComponent<TakeDamage>();
            Vector3 direction = transform.position - hit.transform.position;
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (rb != null && !hit.gameObject.name.Contains("Popper") && !hit.gameObject.name.Contains("Stratum")) {
                rb.AddForce(-direction * rb.mass * Mathf.Sqrt(Physics2D.gravity.magnitude) * radius / distance, ForceMode2D.Impulse);
            }

            if (player != null) {
                player.TakeDamage(Mathf.RoundToInt(250 * radius / distance));
            } else if (creature != null) {
                creature.ReceiveDamage(Mathf.RoundToInt(250 * radius / distance));
            }
        }
        
        detonate = false;
        yield return new WaitForSeconds(explodeSound.length);
        Destroy(gameObject);
    }
 
}

public static class Rigidbody2DExtension {
 
    public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode2D mode = ForceMode2D.Impulse) {
        var explosionDir = rb.position - explosionPosition;
        var explosionDistance = explosionDir.magnitude;

        if (upwardsModifier == 0)
            explosionDir /= explosionDistance;
        else {
            explosionDir.y += upwardsModifier;
            explosionDir.Normalize();
        }

        rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
    }

}
