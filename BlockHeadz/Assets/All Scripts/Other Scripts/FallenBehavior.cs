using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenBehavior : MonoBehaviour {
    
    #region Public Variables
    [SerializeField] float attackDistance;
    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection; 
    #endregion

    #region Private Variables
    private Rigidbody2D rb;
    private float hitCooldown;
    private static readonly float cooldown = 2f;
    private bool hit = true;
    private bool movingLeft = true;
    private float viewFinder;
    private float speed = 5f;
    private float knockbackForce;
    #endregion

    void Start() {
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

        if (movingLeft) {
            viewFinder = .2f;
            knockbackForce = -5f;
        } else {
            viewFinder = .2f;
            knockbackForce = 5f;
        }

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, .1f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
    
        #region String Builder
        string ground;
        string block;

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
        #endregion

        if (block == "Ground" || ground == "Nothing") {
            Flip();
        }

        if (block == "Player" && hit) {
            Hit(50);
        }
    }

    void Flip() {
        if (movingLeft) {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = false;
        } else {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingLeft = true;
        }
    }

    void Hit(int damage) {
        StartCoroutine(GetComponent<FallenCombat>().Hit(damage));
        hit = false;
        hitCooldown = cooldown;
    }
}
