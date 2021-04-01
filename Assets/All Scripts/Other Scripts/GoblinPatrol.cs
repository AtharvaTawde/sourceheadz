using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinPatrol : MonoBehaviour {
    // Put NT tag on anything that should not be detected by a Goblin!!!
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    [SerializeField] Transform backDetection;
    
    private float jumpForce;
    private float speed = 2f;
    private bool movingLeft = true;
    private float viewFinder;
    private Rigidbody2D rb;
    private bool hit = true;

    public static readonly float cooldown = 1f;
    private float hitCooldown;       
 
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        hitCooldown = 0f;
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * 0.5f) * rb.mass;
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
        } else {
            viewFinder = -.2f;
        }

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 7f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);
        RaycastHit2D backInfo = Physics2D.Raycast(backDetection.position, Vector2.right, viewFinder * 7.5f);

        #region String Builder
        bool NameCheck(RaycastHit2D hit2D, string name) {
            return hit2D.collider.gameObject.name.Contains(name);
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

        string Back() {
            if (ColliderCheck(backInfo)) {
                return backInfo.collider.tag;
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

        if ((Block() == "Ground" || Block() == "Block") && Air() == "Nothing" && (Ground() == "Ground" || Ground() == "Block")) {
            Jump();
        }
        
        if (Air() == "Ground" || Air() == "Block" || Air() == "Enemy" || Ground() == "Nothing" || Block() == "Enemy") {
            FlipEnemy();
        }
        
        if (Block() == "Player" && hit && !blockInfo.collider.GetComponent<PlayerMovement>().isDead) {
            if (gameObject.name.Contains("Sobb")) {
                Hit(75);
            } else {
                Hit(38);
            }
        }

        if (Back() == "Player" && !backInfo.collider.GetComponent<PlayerMovement>().isDead) {
            FlipEnemy();
        }
    }

    void FlipEnemy() {
        if (movingLeft) {
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

    void Hit(int damage) {
        StartCoroutine(GetComponent<GoblinEnemy>().Hit(damage));
        hit = false;
        hitCooldown = cooldown;
    }

    private void OnDrawGizmos() {
        Vector3 blockEndPoint = backDetection.position;
        if (movingLeft) { 
            blockEndPoint.x -= (viewFinder * 7.5f); 
        } else if (!movingLeft) {
            blockEndPoint.x += (viewFinder * 7.5f);
        }

        Gizmos.DrawLine(backDetection.position, blockEndPoint); 
    }
}
