using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController2D controller;
    public Animator animator;
    public bool crouch = false;
    public bool isPaused = false;
    public bool isDead = false;
    public bool isInWater;
    public bool sprint = false;
    public bool jump = false;
    public bool isGrounded;
    public GameObject PauseMenu;
    public GameObject DeathMenu;
    public float runSpeed;
    public float horizontalMove = 0f;

    [SerializeField] Vector3 spawnPoint;
    [SerializeField] CraftingSystem cs;   
    [SerializeField] PlayerCombat player;

    private AudioSource audioSource;
    private Vector3 cursorPos; 
    private Rigidbody2D rb;
    private float agilityConstant = 1f;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerCombat>();
        spawnPoint = transform.position;
        DeathMenu.transform.parent.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void Update() {
        cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isInWater = player.isInWater;

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
            agilityConstant = player.agility > 0 ? 1.5f : 1f;
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * agilityConstant;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
            
            if (!isInWater) {
                if (Input.GetButtonDown("Jump")) {
                    jump = true;
                    player.saturation -= 0.2f;
                    animator.SetBool("IsJumping", true);
                }

                crouch = Input.GetButton("Vertical");
                sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                if (crouch) {player.saturation -= 0.1f;} 
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
        foreach (GameObject g in player.playerComponents) {
            g.SetActive(true);
        }
        isDead = false;
        jump = false;
        crouch = false;
        sprint = false;
        player.currentHealth = player.maxHealth;
        player.currentHunger = player.maxHunger;
        player.displayHunger = Mathf.RoundToInt(player.maxHunger);
    }

    public void OnLanding() {
        animator.SetBool("IsJumping", false);
    }

    public void OnCrouching(bool isCrouching) {
        animator.SetBool("IsCrouching", isCrouching);
    } 

    void FixedUpdate() {
        float currentHunger = player.currentHunger;
        float maxHunger = player.maxHunger;
        float swimmingPower = 15f;
        isGrounded = GetComponent<CharacterController2D>().m_Grounded;
        
        if (!isInWater) {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        } else {
            crouch = false;
            sprint = false;
            jump = false;
            controller.Move(horizontalMove / 2 * Time.fixedDeltaTime, false, false);

            if (Input.GetButton("Jump")) { // Jump
                rb.AddForce(Vector2.up * swimmingPower);

                if (rb.velocity.y > 15) {
                    rb.velocity = new Vector2(rb.velocity.x, 15f);
                }

                player.saturation -= 0.4f;
            }

            if (Input.GetButton("Vertical")) { // Crouch
                rb.AddForce(Vector2.down * swimmingPower);
                
                if (rb.velocity.y < -15) {
                    rb.velocity = new Vector2(rb.velocity.x, -15f);
                }
                
                player.saturation -= 0.4f;
            } 
        }
        
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