using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NyertPatrol : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    [SerializeField] float viewFinder;
    [SerializeField] float forwardForce;
    [SerializeField] float jumpForce = 250f;
    Rigidbody2D rb;
    private bool movingLeft = true;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = transform.GetChild(0);
        blockDetection = transform.GetChild(1);
        airDetection = transform.GetChild(2);
    }

    private void OnValidate() {
        groundDetection = transform.GetChild(0);
        blockDetection = transform.GetChild(1);
        airDetection = transform.GetChild(2);
    }
     
    void Update() {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (movingLeft) {
            viewFinder = .2f;
            forwardForce = -2f;
        } else {
            viewFinder = -.2f;
            forwardForce = 2f;
        }
        
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        # region String Builder
        string air;
        string block;
        string ground;

        if (airInfo.collider) {
            air = airInfo.collider.tag;  
        } else {
            air = "Nothing";
        }

        if (blockInfo.collider) {
            block = blockInfo.collider.tag;
        } else {
            block = "Nothing";
        }

        if (groundInfo.collider) {
            ground = groundInfo.collider.tag;
        } else {
            ground = "Nothing";
        }

        Debug.Log("Ground: " + ground );
        #endregion

        if (ground == "Nothing" || block == "Player" || block == "Health Pot" || (block == "Enemy" && blockInfo.collider.gameObject != gameObject) || air == "Ground" ) {
            FlipEnemy();
        }

        if ((block == "Ground" || block == "Bounce Pad") && air == "Nothing") {
            Jump();
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

    void Jump() {
        rb.AddForce(new Vector2(forwardForce, jumpForce));
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
