using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinEnemy : MonoBehaviour {
    public Animator animator;
    public int maxHealth;
    public int currentHealth;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask pLayer;
    GameObject Explosion;
    CameraShake cameraShake;
    [SerializeField] GameObject goblinMeat;
    [SerializeField] GameObject woodShard;
    [SerializeField] GameObject eyeBall;
    int meatDropPercentChance = 40;
    int woodShardPercentChance = 15;
    int eyeBallPercentChance = 2;
    AudioSource audioSource;
    AudioClip hitSound;

    private void OnValidate() {
        audioSource = GetComponent<AudioSource>();
        hitSound = Resources.Load("hit") as AudioClip;
        currentHealth = maxHealth;
        goblinMeat = Resources.Load("Goblin Meat") as GameObject;
        woodShard = Resources.Load("Wood Shard") as GameObject;
        eyeBall = Resources.Load("Eyeball") as GameObject;
        Explosion = Resources.Load("Blood") as GameObject;
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
        hitSound = Resources.Load("hit") as AudioClip;
        currentHealth = maxHealth;
        goblinMeat = Resources.Load("Goblin Meat") as GameObject;
        woodShard = Resources.Load("Wood Shard") as GameObject;
        eyeBall = Resources.Load("Eyeball") as GameObject;
        Explosion = Resources.Load("Blood") as GameObject;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        int random = Mathf.RoundToInt(Random.Range(0, 100));
        animator.SetTrigger("Hurt");
        if (currentHealth <= 0) {
            StartCoroutine(Blood());
            if (gameObject.name.Contains("Goblin") && random < meatDropPercentChance) {
                Instantiate(goblinMeat, transform.position, transform.rotation);
            }

            if (gameObject.name.Contains("Goblin") && random < woodShardPercentChance) {
                Instantiate(woodShard, transform.position, transform.rotation);
            }

            if (gameObject.name.Contains("Zombi") && random < eyeBallPercentChance) {
                Instantiate(eyeBall, transform.position, transform.rotation);
            }

            Die();
        }
    }

    private void Update() {
        cameraShake = GameObject.Find("Camera Container/Main Camera").GetComponent<CameraShake>();
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
        
        if (gameObject.name.Contains("Goblin")) {
            PlayerPrefsHelper.IncrementInt("Goblins Killed");
        } else if (gameObject.name.Contains("Zombi")) {
            PlayerPrefsHelper.IncrementInt("Zombies Killed");
        }

        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

}

public static class PlayerPrefsHelper {
    
    public static void IncrementInt(string key) {
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
    }

}
