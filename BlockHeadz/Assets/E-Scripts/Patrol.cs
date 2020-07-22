﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
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
    private float jumpForce = 250f;

 
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
     
    void Update() {
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

        void FlipEnemy() {
            if (movingLeft == true) {
                transform.eulerAngles = new Vector3(0, 0, 0);
                movingLeft = false;
            } else {
                transform.eulerAngles = new Vector3(0, -180, 0);
                movingLeft = true;
            }
        }

        if (rb.simulated == true) {
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 2f);
            if (groundInfo.collider == false) {
                FlipEnemy();
            }
            RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
            RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);
            if (blockInfo.collider == true && airInfo.collider == false) {
                if (blockInfo.collider.tag != "Enemy" && blockInfo.collider.tag != "Player") {
                    rb.AddForce(new Vector2(0f, jumpForce));
                } else {
                    FlipEnemy();
                }
            } else if (airInfo.collider == true) {
                FlipEnemy();
            }
        }
    }
}
