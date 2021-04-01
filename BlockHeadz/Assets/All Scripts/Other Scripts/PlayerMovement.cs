using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController2D controller;
    public Animator animator;
    public bool crouch = false;
    public bool isPaused = false;
    public bool isDead = false;
    public GameObject PauseMenu;
    public GameObject DeathMenu;
    public float runSpeed;
    public float horizontalMove = 0f;
    public bool sprint = false;
    public bool jump = false;
    public bool isGrounded;

    [SerializeField] Vector3 spawnPoint;
    [SerializeField] CraftingSystem cs;   

    private AudioSource audioSource;
    private Vector3 cursorPos; 

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        spawnPoint = transform.position;
        DeathMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void Update() {
        cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (isDead && DeathMenu.transform.localScale == Vector3.zero) {
            DeathMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
            DeathMenu.transform.localScale = Vector3.one;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused && !isDead) {
            PauseMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = true;
            PauseMenu.transform.localScale = Vector3.one;
            audioSource.Stop();
            Time.timeScale = 0f;
            isPaused = true;
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) {
            PauseMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
            PauseMenu.transform.localScale = Vector3.zero;
            audioSource.Play();
            Time.timeScale = 1f;
            isPaused = false;
        }

        if (!isPaused && !isDead && !cs.isCraftingMenuActive) {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            
            if (Input.GetButtonDown("Jump")) {
                jump = true;
                GetComponent<PlayerCombat>().saturation -= 0.2f;
                animator.SetBool("IsJumping", true);
            }

            if (Input.GetButtonDown("Vertical")) { // crouch.
                crouch = true;
                GetComponent<PlayerCombat>().saturation -= 0.1f;
            } else if (Input.GetButtonUp("Vertical")) {
                crouch = false;
            }   

            if (Input.GetKeyDown(KeyCode.LeftShift)) { // Sprint.
                sprint = true;
            } else if (Input.GetKeyUp(KeyCode.LeftShift)) {
                sprint = false;
            }
        }

        if (cs.isCraftingMenuActive) {
            horizontalMove = 0;
        }
    }

    public void Respawn() {
        transform.position = spawnPoint;
        DeathMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
        DeathMenu.transform.localScale = Vector3.zero;
        GetComponent<SpriteRenderer>().enabled = true;
        foreach (GameObject g in GetComponent<PlayerCombat>().playerComponents) {
            g.SetActive(true);
        }
        isDead = false;
        jump = false;
        crouch = false;
        sprint = false;
        GetComponent<PlayerCombat>().currentHealth = GetComponent<PlayerCombat>().maxHealth;
        GetComponent<PlayerCombat>().currentHunger = GetComponent<PlayerCombat>().maxHunger;
        GetComponent<PlayerCombat>().displayHunger = Mathf.RoundToInt(GetComponent<PlayerCombat>().maxHunger);
    }

    public void OnLanding() {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching) {
        animator.SetBool("IsCrouching", isCrouching);
    } 

    void FixedUpdate() {
        float currentHunger = GetComponent<PlayerCombat>().currentHunger;
        float maxHunger = GetComponent<PlayerCombat>().maxHunger;
        isGrounded = GetComponent<CharacterController2D>().m_Grounded;
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;

        if (currentHunger / maxHunger > 0.3f) {
            if (sprint && !isGrounded) {
                runSpeed = 62.5f * 1.1f;
            } else if (sprint && isGrounded) {
                runSpeed = 62.5f;
            } else if (!sprint && !isGrounded) {
                runSpeed = 31.25f * 1.1f;
            } else if (!sprint && isGrounded) {
                runSpeed = 31.25f;
            }
        } else {
            runSpeed = 31.25f * ((currentHunger / maxHunger) / 0.3f);
        }

        if (GetComponent<ItemSelection>().eatTime > 0 && GetComponent<ItemSelection>().eatTime < 1) {
            runSpeed = 15f;
        }

    }
}