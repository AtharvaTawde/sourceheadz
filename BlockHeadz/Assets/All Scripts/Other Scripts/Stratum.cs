using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class Stratum : MonoBehaviour {
    
    public int currentHealth;
    public int maxHealth;
    public int form = 1;
    
    [SerializeField] float randomX; 
    [SerializeField] Transform target;
    [SerializeField] Transform throwingArm; 
    [SerializeField] Transform throwPoint; 
    [SerializeField] Transform rockToThrow;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject rock; 
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject triangleProj;
    [SerializeField] BoxCollider2D[] boxes;

    private int range = 25;
    private float blowRange = 10f;
    private float distance;
    private float fireTimer = 0; 
    private float moveTimer = 0; 
    private float contactCooldown = 0;
    private bool doDamage = false;
    private bool convertToModeAlpha = true;
    private bool formRock = true; 
    private GameObject dealDamageTo; 
    private Rigidbody2D rb;
    private Vector3 actualTarget;
    
    public Animator animator;

    private Dictionary<string, int> drops = new Dictionary<string, int> {
        {"Chicken", 100}
    };

    private Dictionary<string, int> burnDrops = new Dictionary<string, int> {
        {"Cooked Chicken", 100}
    };

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Update() {
        actualTarget = target.position + new Vector3(0, -3, 0);
        distance = Vector3.Distance(actualTarget, transform.position);
        float percentageHealth = (float)currentHealth / (float)maxHealth;

        if (distance < 55 && !healthBar.activeSelf && currentHealth > 0) {
            healthBar.SetActive(true);
        } else if (distance > 55 && healthBar.activeSelf) {
            healthBar.SetActive(false);
        }

        #region Mode Selector 

        //  [50% - 100% Health] = Rock Monster Mode
        //  [25% - 50% Health] = Static Assault Drone
        //  [0% - 25% Health] = Dynamic Assault Drone; Stalagmites Fall and Crush Everything Underneath Them

        if (percentageHealth.IsWithin(0.25f, 0.5f)) {
            form = 2;   
            if (convertToModeAlpha)
                StartCoroutine(ModeAlpha(Vector3.one, Vector3.zero, new GameObject[] {transform.GetChild(0).gameObject, transform.GetChild(1).gameObject, transform.GetChild(2).gameObject}));

            while (transform.position.y < 1373) {
                transform.Translate(Vector2.up);
            }

            if (doDamage && contactCooldown <= 0) {
                DealContactDamage(dealDamageTo);
            } else if (doDamage && contactCooldown > 0) {
                contactCooldown -= Time.deltaTime;
            }

            if (distance < 55 && fireTimer == 0 && !convertToModeAlpha && rb.velocity == Vector2.zero) {
                ActualFire();
                fireTimer = 5f;
            }
        } else if (percentageHealth < 0.25f) {

            if (doDamage && contactCooldown <= 0) {
                DealContactDamage(dealDamageTo);
            } else if (doDamage && contactCooldown > 0) {
                contactCooldown -= Time.deltaTime;
            }

            if (distance < 55 && fireTimer == 0 && !convertToModeAlpha && rb.velocity == Vector2.zero) {
                ActualFire();
                fireTimer = 5f;
            }

            if (moveTimer == 0) {
                MoveAround();
                moveTimer = 5f;
            }

        } else {

            form = 1; 
            if (distance.IsWithin(blowRange, range)) {
                animator.SetBool("Blow", false);
                animator.SetBool("Throw", true);
            } else if (distance < blowRange) {
                animator.SetBool("Throw", false);
                animator.SetBool("Blow", true);
            } else if (distance > range) {
                animator.SetBool("Throw", false);
                animator.SetBool("Blow", false);
                animator.SetTrigger("Idle");
            }

        }
        #endregion

        fireTimer = fireTimer > 0 ? fireTimer -= Time.deltaTime : fireTimer = 0;
        moveTimer = moveTimer > 0 ? moveTimer -= Time.deltaTime : moveTimer = 0;
        transform.eulerAngles = target.position.x < transform.position.x ? transform.eulerAngles = Vector3.zero : transform.eulerAngles = new Vector3(0, 180, 0);

        if (target.GetComponent<PlayerMovement>().isDead) {
            animator.SetBool("Blow", false);
            animator.SetBool("Throw", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        doDamage = true;
        dealDamageTo = other.gameObject;

        if (other.gameObject.tag == "Block") {
            other.gameObject.GetComponent<TakeDamage>().BlockDrop();
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        doDamage = false;
        dealDamageTo = null;     
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Die() {
        HelperFunctions.IncrementInt("Bosses Killed");
        GenerateDrops();
        Destroy(gameObject);
    }

    // Must run first
    void CreateRock() {
        if (formRock) {
            formRock = false;
            GameObject rockInstance = Instantiate(rock, throwPoint.position, throwPoint.rotation);
            rockInstance.transform.parent = throwingArm;
            rockToThrow = rockInstance.transform;
        }   
    }

    // Must run second
    void ThrowRock() {
        Vector3 direction = actualTarget - transform.position;
        if (rockToThrow != null) {
            rockToThrow.transform.SetParent(null); 
            rockToThrow.GetComponent<Rigidbody2D>().AddForce(direction * rockToThrow.GetComponent<Rigidbody2D>().mass * Mathf.Sqrt(Physics2D.gravity.magnitude) * range / distance, ForceMode2D.Impulse);
        }
        formRock = true;
    }

    void BlowAllObjectsAway() {
        Collider2D[] objectsInFront = Physics2D.OverlapCircleAll(transform.position, blowRange);
        foreach (Collider2D c in objectsInFront) {
            Vector3 direction = transform.position - c.transform.position;
            if (c.attachedRigidbody != null) {
                c.attachedRigidbody.AddForce(-direction * c.attachedRigidbody.mass * 500f);
            }
        }
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

    IEnumerator ModeAlpha(Vector3 startScale, Vector3 finalScale, GameObject[] things) {
        animator.SetBool("Throw", false);
        animator.SetBool("Blow", false);
        
        foreach (BoxCollider2D box in boxes) {
            if (box != null) {
                Destroy(box);
            }
        }

        if (throwPoint != null)
            Destroy(throwPoint.gameObject);

        foreach (GameObject thing in things) {
            float elapsedTime = 0f;
            float waitTime = 0.5f;

            while (elapsedTime < waitTime) {
                thing.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime; 
                yield return null;
            }

            thing.transform.localScale = finalScale;
            yield return null;
        }

        foreach (GameObject thing in things) {
            Destroy(thing);
        }

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        convertToModeAlpha = false;
    }

    IEnumerator Fire() {
        for (int i = 0; i < 4; i++) {
            Vector3 direction = actualTarget - transform.position;
            GameObject triInstance = Instantiate(triangleProj, firePoint.position, firePoint.rotation);
            triInstance.GetComponent<Rigidbody2D>().AddForce(direction * 100f);
            triInstance.transform.rotation = Quaternion.Slerp(triInstance.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ActualFire() {
        StartCoroutine(Fire());
    }

    void DealContactDamage(GameObject entity) {
        if (entity.GetComponent<TakeDamage>() != null) {
            entity.GetComponent<TakeDamage>().ReceiveDamage(10);
        } else if (entity.tag == "Player") {
            entity.GetComponent<PlayerCombat>().TakeDamage(10);
        } else if (entity.tag == "Enemy") {
            string[] enemyNames = {"Nyert", "Goblin", "Zombi", "Archer", "Fallen", "Chicken", "Revenant"};
            foreach (string n in enemyNames) {
                if (entity.name.Contains(n)) {
                    Vector3 direction = entity.transform.position - transform.position;
                    entity.GetComponent<Rigidbody2D>().AddForce(direction * rb.mass * 150f); 
                    entity.GetComponent<TakeDamage>().ReceiveDamage(10);
                }
            }
        }
        contactCooldown = 0.25f;
    }

    void MoveAround() {
        randomX = Random.Range(1191, 1241);
        Vector3 initPos = transform.position;
        StartCoroutine(MoveTowards(transform, initPos, new Vector3(randomX, initPos.y, 0)));
        Debug.Log("Moved");
    }

    IEnumerator MoveTowards(Transform thing, Vector3 startPos, Vector3 finalPos) {
        float elapsedTime = 0f;
        float waitTime = 1f;

        while (elapsedTime < waitTime) {
            thing.position = Vector3.Lerp(startPos, finalPos, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.position = finalPos;
        yield return null;
    }

}

