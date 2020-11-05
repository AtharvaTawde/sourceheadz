using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAttack : MonoBehaviour
{
    #region Public Variables
    public Transform raycast;
    public LayerMask raycastMask;
    public GameObject goblinHorde;
    #endregion

    #region Private Variables
    private float rayCastLength;
    private float attackDistance;
    private RaycastHit2D hit;
    private Transform target;
    private Animator anim;
    private Rigidbody2D rb;
    private float distance;
    private bool inRange;
    private bool isAttacking;
    private Vector2 v;
    #endregion

    void Awake() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        attackDistance = 5f;
        rayCastLength = 7f;
    }

    void Update() {
        Flip();
        bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("Disciple_Attack");

        if (inRange) {
            hit = Physics2D.Raycast(raycast.position, v, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        if (hit.collider != null) {
            EnemyLogic();
        } else if (hit.collider == null) {
            inRange = false;
        }

        if (!inRange) {
            StopAttack();
        } 
    }

    void OnTriggerEnter2D(Collider2D trig) {
        if (trig.gameObject.tag == "Player") {
            target = trig.transform;
            inRange = true;
        }
    }

    void EnemyLogic() {
        distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackDistance) {
            StopAttack();
        } else if (attackDistance >= distance) {
            Attack();
        } 
    }

    void Attack() {
        anim.SetBool("Attack", true);
        goblinHorde.SetActive(true);
        anim.SetTrigger("Fade");
        rb.AddForce(new Vector2(0, 1000));
        rb.simulated = false;
        Destroy(gameObject, 10);
    }

    void StopAttack() {
        anim.SetBool("Attack", false);
    }

    void RaycastDebugger() {
        if (distance > attackDistance) {
            Debug.DrawRay(raycast.position, v * rayCastLength, Color.red);
        } else if (attackDistance > distance) {
            Debug.DrawRay(raycast.position, v * rayCastLength, Color.green);
        }
    }

    private void Flip() {
        if (target != null && transform.position.x > target.position.x) {
            v = Vector2.left;
        } else if (target.position.x > transform.position.x) {
            v = Vector2.right;
        }
    }
}
