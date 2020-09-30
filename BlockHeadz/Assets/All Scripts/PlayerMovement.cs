using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 100f;
    public Rigidbody2D rb;
    float horizontalMove = 0f;
    bool sprint = false;
    bool jump = false;
    public bool crouch = false;
    public bool isPaused = false;
    GameObject PauseMenu;

    private void Start() {
        PauseMenu = GameObject.Find("UI/Pause Menu");
        PauseMenu.SetActive(false);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused && !GetComponent<PlayerCombat>().dead) {
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) {
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }

        if (!isPaused)
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            if (Input.GetButtonDown("Jump")) {
                jump = true;
                animator.SetBool("IsJumping", true);
            }

            if (Input.GetButtonDown("Vertical")) {
                crouch = true;
            } else if (Input.GetButtonUp("Vertical")) {
                crouch = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) { // Sprint.
                sprint = true;
            } else if (Input.GetKeyUp(KeyCode.LeftShift)) {
                sprint = false;
            }

            if (Input.GetKeyDown(KeyCode.R)) { // Teleport back to the beginning. 
                Reset();
            }
    }

    public void OnLanding() {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching) {
        animator.SetBool("IsCrouching", isCrouching);
    } 

    void FixedUpdate() {
        
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;

        if (sprint) {
            runSpeed = 150f;
        } else if (!sprint) {
            runSpeed = 100f;
        }
    }

    public void Reset() {
        transform.position = new Vector3(-7, 0, 0);
    }    

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Reset")
            Reset();
    }
}