using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NyertPatrol : MonoBehaviour  {
    // Put NT tag on anything that should not be detected by a Nyert!!!
    [SerializeField] float speed;
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    [SerializeField] float viewFinder;
    [SerializeField] float jumpForce;
    Rigidbody2D rb;
    private bool movingLeft = true;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundDetection = transform.GetChild(0);
        blockDetection = transform.GetChild(1);
        airDetection = transform.GetChild(2);
        jumpForce = Mathf.Sqrt(Physics2D.gravity.magnitude * rb.gravityScale / 16) * rb.mass;
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
        } else {
            viewFinder = -.2f;
        }
        
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        # region String Builder

        bool NameCheck(RaycastHit2D hit2D, string name) {
            if (hit2D.collider.gameObject.name.Contains(name)) {
                return true;
            } else {
                return false;
            }
        }  

        bool ColliderCheck(RaycastHit2D hit) {
            return hit.collider && 
                   hit.collider.tag != "NT" && 
                   hit.collider.tag != "Water" && 
                   !NameCheck(hit, "Gunflower") && 
                   !NameCheck(hit, "Rock") && 
                   !NameCheck(hit, "Health Pot") && 
                   hit.collider.tag != "Item";
        }

        string Air() {
            if (ColliderCheck(airInfo)) {
                return airInfo.collider.tag;  
            } else {
                return "Nothing";
            }
        }

        string Block() {
            if (ColliderCheck(blockInfo)) {
                return blockInfo.collider.tag;
            } else {
                return "Nothing";
            }
        }

        string Ground() {
            if (groundInfo.collider) {
                return groundInfo.collider.tag;
            } else {
                return "Nothing";
            }
        }

        #endregion

        if (Ground() == "Nothing" || Air() == "Block" || Block() == "Player" || Block() == "Health Pot" || (Block() == "Enemy" && blockInfo.collider.gameObject != gameObject) || Air() == "Ground") {
            FlipEnemy();
        }

        if ((Block() == "Ground" || Block() == "Bounce Pad" || Block() == "Block") && Air() == "Nothing") {
            Jump();
        }
    }

    void FlipEnemy() {
        if (movingLeft == true) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = false;
        } else {
            transform.eulerAngles = new Vector3(0, 180, 0);
            movingLeft = true;
        }
    }

    void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
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
