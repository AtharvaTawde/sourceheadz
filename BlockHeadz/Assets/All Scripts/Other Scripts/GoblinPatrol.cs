using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPatrol : MonoBehaviour {
    
    private float speed = 2f;
    private bool movingLeft = true;
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    private float viewFinder;
    private float forwardForce;
    private Rigidbody2D rb;
    [SerializeField] float jumpForce;
    private bool hit = true;

    public static readonly float cooldown = 1f;
    private float hitCooldown;       
 
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        hitCooldown = 0f;
    }
     
    private void Update() {
        if (hitCooldown <= 0) {
            hit = true;
        } else {
            hit = false;
            hitCooldown -= Time.deltaTime;
        }   

        if (movingLeft) {
            viewFinder = .2f;
            forwardForce = -2f;
        } else {
            viewFinder = -.2f;
            forwardForce = 2f;
        }

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 7f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        #region String Builder
        string air;
        string block;
        string ground;

        if (blockInfo.collider) {
            block = blockInfo.collider.tag;
        } else {
            block = "Nothing";
        }

        if (airInfo.collider) {
            air = airInfo.collider.tag;
        } else {
            air = "Nothing";
        }

        if (groundInfo.collider) {
            ground = groundInfo.collider.tag;
        } else {
            ground = "Nothing";
        }
        #endregion

        if (block == "Ground" && air == "Nothing" && ground == "Ground") {
            Jump();
        }
        
        if (air == "Ground" || ground == "Nothing" || block == "Enemy") {
            FlipEnemy();
        }
        
        if (block == "Player" && hit) {
            Hit(38);
        }
    }

    void FlipEnemy() {
        if (movingLeft) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = false;
        } else {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingLeft = true;
        }
    }

    void Jump() {
        rb.AddForce(new Vector2(forwardForce, jumpForce));
    }

    void Hit(int damage) {
        StartCoroutine(GetComponent<GoblinEnemy>().Hit(damage));
        hit = false;
        hitCooldown = cooldown;
    }

    private void OnDrawGizmos() {
        Vector3 blockEndPoint = blockDetection.position;
        if (movingLeft) { 
            blockEndPoint.x -= viewFinder; 
        } else if (!movingLeft) {
            blockEndPoint.x += viewFinder;
        }

        Gizmos.DrawLine(blockDetection.position, blockEndPoint); 
    }
}
