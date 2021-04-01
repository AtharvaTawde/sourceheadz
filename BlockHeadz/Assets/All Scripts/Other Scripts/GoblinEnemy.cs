using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class GoblinEnemy : MonoBehaviour {
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask pLayer;
    public float burnTime = 0f;

    [SerializeField] GameObject onFireGraphic;
    
    private GameObject Explosion;
    private CameraShake cameraShake;
    private float burnCooldown = 0f;
    private AudioSource audioSource;
    private AudioClip hitSound;
    private bool hurt;
    private Rigidbody2D rb;

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"Goblin Meat", 50},
        {"Wood Shard", 45}
    };

    private Dictionary<string, int> burnDrops = new Dictionary<string, int> {
        {"Cooked Goblin Meat", 50},
        {"Wood Shard", 45}
    };

    void Start() {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        burnCooldown = 0f;
        hitSound = Resources.Load("hit") as AudioClip;
        currentHealth = maxHealth;
        Explosion = Resources.Load("Blood") as GameObject;    
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        int random = Mathf.RoundToInt(Random.Range(0, 100));
        hurt = true;
        if (currentHealth <= 0) {
            StartCoroutine(Blood());
            Die();
        }
    }

    private void Update() {
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();
        Burn();
        if (hurt) {
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        }
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

    public IEnumerator Hit(int damage) {
        float random = Random.Range(.5f, 1f);
        audioSource.PlayOneShot(hitSound, 1f);
        animator.SetBool("Hit", true);
        StartCoroutine(cameraShake.Shake(.15f, .05f));
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, pLayer);
        yield return new WaitForSeconds(.5f);
        foreach (Collider2D player in hitPlayer) {
            player.GetComponent<PlayerCombat>().TakeDamage(damage);
        }
        yield return new WaitForSeconds(.25f);
        animator.SetBool("Hit", false);
    }

    public void Die() {
        HelperFunctions.IncrementInt("Goblins Killed");
        GenerateDrops();
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    void GenerateDrops() {
        if (burnTime > 0) {
            foreach (KeyValuePair<string, int> entry in burnDrops) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= entry.Value) {
                    Drop(RemoveIntegers(entry.Key), 1);
                }
            }
        } else {
            foreach (KeyValuePair<string, int> entry in drops) {
                int random = Mathf.RoundToInt(Random.Range(0, 100));

                if (random <= entry.Value) {
                    Drop(RemoveIntegers(entry.Key), 1);
                }
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

    string RemoveIntegers(string input) {
        return Regex.Replace(input, @"[\d-]", string.Empty);
    }

}