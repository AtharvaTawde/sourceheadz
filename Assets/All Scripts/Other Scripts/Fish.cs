using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fish : MonoBehaviour {
    
    [SerializeField] int currentHealth;
    
    public Collider2D AgentCollider { get { return agentCollider; } }

    private Collider2D agentCollider;
    private GameObject Explosion; 
    private float speed = 6;
    private int maxHealth = 150;
    private bool hurt; 

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"Clownfish", 50},
    };


    void Start() {
        agentCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        Explosion = Resources.Load("Blood") as GameObject;
    }

    private void Update() {
        if (hurt) {
            StartCoroutine(GetComponent<HitEffect>().HurtEffect());
            hurt = false;
        }
    }

    public void Move(Vector2 velocity) {
        transform.right = -velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;
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

    public void Die() {
        HelperFunctions.IncrementInt("Fish Killed");
        GenerateDrops();
        Destroy(gameObject);
    }

    IEnumerator Blood() {
        GameObject bloodInstance = Instantiate(Explosion, transform.position, transform.rotation) as GameObject;
        yield return new WaitForSeconds(10f);
        Destroy(bloodInstance);
    }

    void GenerateDrops() {
        foreach (KeyValuePair<string, int> entry in drops) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= entry.Value) {
                Drop(entry.Key.RemoveIntegers(), 1);
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

}
