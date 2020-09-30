using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPatrol : MonoBehaviour
{
    public float speed;
    public float distance;
    private bool movingLeft = true;
    public Transform groundDetection;
    public Transform blockDetection;
    public Transform airDetection;
    public float viewFinder;
    public float forwardForce;
    private Rigidbody2D rb;
    public float jumpForce = 20000f;
    public bool hit = true;

    public static readonly float cooldown = 1f;
    public float hitCooldown;
 
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        hitCooldown = 0f;
    }
     
    void Update() {
        if (hitCooldown <= 0) {
            hit = true;
        } else {
            hit = false;
            hitCooldown -= Time.deltaTime;
        }   

        if (rb.simulated == false) {
            speed = 0f;
        } else {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }

        if (movingLeft) {
            viewFinder = 0.000001f;
            forwardForce = -2f;
        } else {
            viewFinder = -0.000001f;
            forwardForce = 2f;
        }

        if (rb.simulated) {
            if (groundDetection != null && blockDetection != null && airDetection != null) {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 7f);
                RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
                RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

                if (groundInfo.collider == false) {
                    FlipEnemy();
                }          

                if (blockInfo.collider && airInfo.collider == false) {
                    if (blockInfo.collider.tag != "Enemy" && blockInfo.collider.tag != "Player") {
                        rb.AddForce(new Vector2(forwardForce, jumpForce));
                    }
                } else if (airInfo.collider.tag != "Player") {
                    FlipEnemy();
                }

                if (blockInfo.collider.tag == "Player" && hit) {
                    Hit(10);
                }
            }
        }
    }

    void FlipEnemy() {
        if (movingLeft == true) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = false;
        } else {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingLeft = true;
        }
    }

    void Hit(int damage) {
        StartCoroutine(GetComponent<GoblinEnemy>().Hit(damage));
        hit = false;
        hitCooldown = cooldown;
    }
}
