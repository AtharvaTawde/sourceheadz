using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed;
    float horizontalMove = 0f;
    bool sprint = false;
    bool jump = false;
    public bool crouch = false;
    public bool isPaused = false;
    public GameObject PauseMenu;
    AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        PauseMenu.SetActive(false);
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused && !GetComponent<PlayerCombat>().dead) {
            PauseMenu.SetActive(true);
            audioSource.Stop();
            Time.timeScale = 0f;
            isPaused = true;
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) {
            PauseMenu.SetActive(false);
            audioSource.Play();
            Time.timeScale = 1f;
            isPaused = false;
        }

        if (!isPaused) {
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
            runSpeed = 112.5f;
        } else if (!sprint) {
            runSpeed = 62.5f;
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