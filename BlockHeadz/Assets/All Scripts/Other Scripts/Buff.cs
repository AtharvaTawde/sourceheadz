using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour {

    public Animator animator;
    public int currentHealth;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask pLayer;
    public float burnTime = 0f;

    [SerializeField] GameObject onFireGraphic;
    [SerializeField] Transform target;
    [SerializeField] Transform player;
    [SerializeField] float distance;

    private GameObject Explosion;
    private CameraShake cameraShake;
    private float burnCooldown = 0f;
    private int maxHealth = 1500;
    private AudioSource audioSource;
    private AudioClip hitSound;
    private bool hurt;
    private Rigidbody2D rb;

    [SerializeField] Transform groundDetection;
    [SerializeField] Transform blockDetection;
    [SerializeField] Transform airDetection;
    
    private float jumpForce;
    private float speed = 0f;
    private bool movingLeft = true;
    private float viewFinder;
    private bool hit = true;

    public static readonly float cooldown = 1f;
    private float hitCooldown;       

    private string[] dropNames = {};
    private int[] dropRates = {};

    void Start() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        burnCooldown = 0f;
        hitSound = Resources.Load("hit") as AudioClip;
        currentHealth = maxHealth;
        Explosion = Resources.Load("Blood") as GameObject;   
        hitCooldown = 0f;
        jumpForce = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * 0.5f) * rb.mass;
        player = target;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        hurt = true;
        if (currentHealth <= 0) {
            StartCoroutine(Blood());
            Die();
        }
    }
    
    void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Update() {
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();
        float renderDistance = Screen.width / 2;
        Burn();
        if (hurt) {
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        }

        if (target != null) {
            distance = Vector3.Distance(target.position, transform.position);
            if (target.position.x < transform.position.x) {
                transform.eulerAngles = new Vector3(0, 180, 0);
            } else {
                transform.eulerAngles = Vector3.zero;
            }
        } else {
            distance = 1f;
        }

        if (distance < renderDistance) {
            speed = 4f;
            animator.SetBool("Walk", true);
        } else {
            speed = 0f;
            animator.SetBool("Walk", false);
        }

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

        transform.Translate(Vector2.right * speed * Time.deltaTime);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, 7f);
        RaycastHit2D blockInfo = Physics2D.Raycast(blockDetection.position, Vector2.left, viewFinder);
        RaycastHit2D airInfo = Physics2D.Raycast(airDetection.position, Vector2.left, viewFinder);

        #region String Builder
        string air;
        string block;
        string ground;

        bool NameCheck(RaycastHit2D hit2D, string name) {
            if (hit2D.collider.gameObject.name.Contains(name)) {
                return true;
            } else {
                return false;
            }
        }  

        if (blockInfo.collider && blockInfo.collider.tag != "NT" && blockInfo.collider.tag != "Water" && !NameCheck(blockInfo, "Gunflower") && !NameCheck(blockInfo, "Rock")) {
            block = blockInfo.collider.tag;
        } else {
            block = "Nothing";
        }

        if (airInfo.collider && airInfo.collider.tag != "NT" && airInfo.collider.tag != "Water" && !NameCheck(airInfo, "Gunflower") && !NameCheck(airInfo, "Rock")) {
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

        if ((block == "Ground" || block == "Block") && air == "Nothing" && (ground == "Ground" || ground == "Block")) {
            target = player;
            Jump();
        }
        
        if (air == "Ground" || air == "Block" || air == "Enemy" || ground == "Nothing" || block == "Enemy") {
            target = null;
            FlipEnemy();
        }
        
        if (block == "Player" && hit && !blockInfo.collider.GetComponent<PlayerMovement>().isDead) {
            target = player;
            Hit(150);
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


    void Hit(int damage) {
        StartCoroutine(CoreHit(damage));
        hit = false;
        hitCooldown = cooldown;
    }

    void Burn() {
        if (burnTime > 0) {
            onFireGraphic.SetActive(true);
            burnTime -= Time.deltaTime;

            if (burnCooldown <= 0) {
                TakeDamage(10);                
                burnCooldown = 0.5f;
            } else {
                burnCooldown -= Time.deltaTime;
            }
        } else {
            onFireGraphic.SetActive(false);
            burnTime = 0;
        }
    }

    public IEnumerator CoreHit(int damage) {
        float random = Random.Range(.5f, 1f);
        animator.SetBool("Hit", true);
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, pLayer);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(cameraShake.Shake(.15f, .05f));
        audioSource.PlayOneShot(hitSound, 1f);
        foreach (Collider2D p in hitPlayer) {
            p.GetComponent<PlayerCombat>().TakeDamage(damage);
        }
        animator.SetBool("Hit", false);
    }

    public void Die() {
        HelperFunctions.IncrementInt("Buffs Killed");
        GenerateDrops();
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    void GenerateDrops() {
        for (int i = 0; i < dropNames.Length; i++) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= dropRates[i]) {
                Drop(dropNames[i], 1);
            }
        }
    }

    void Drop(string name, int amount) {
        for (int i = 0; i < amount; i++) {
            GameObject drop = Resources.Load("Physical Items/" + name) as GameObject;
            GameObject dropInstance = Instantiate(drop, transform.position, transform.rotation);
            dropInstance.name = string.Format("{0}x{1}", name, amount.ToString());
        }
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
