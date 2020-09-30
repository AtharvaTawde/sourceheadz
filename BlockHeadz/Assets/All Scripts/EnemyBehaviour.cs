using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    
    #region Public Variables
    public Transform raycast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    public Transform leftLimit;
    public Transform rightLimit;
    #endregion

    #region Private Variables
    private RaycastHit2D hit;
    private Transform target;
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    private float intTimer;
    private bool movingLeft;
    private Vector2 vector;
    private bool isAttacking;
    #endregion

    void Awake() {
        SelectTarget();
        intTimer = timer;
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (!attackMode) {
            Move();
        }

        bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("Fallen_Attack");

        if (!InsideOfLimits() && !inRange && !isAttacking) {
            SelectTarget();
        }

        if (movingLeft) {
            vector = Vector2.left;
        } else {
            vector = Vector2.right;
        }

        if (inRange) {
            hit = Physics2D.Raycast(raycast.position, vector, rayCastLength, raycastMask);
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
            Flip();
        }
    }

    void EnemyLogic() {
        distance = Vector2.Distance(transform.position, target.position);
        if (distance > attackDistance) {
            StopAttack();
        } else if (attackDistance >= distance && !cooling) {
            Attack();
        } 

        if (cooling) {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    void Move() {
        anim.SetBool("canWalk", true);
        if (!isAttacking) {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack() {
        timer = intTimer;
        attackMode = true;
        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
        GetComponent<FallenCombat>().Hit(Mathf.FloorToInt(Random.Range(25f, 45f)));
    }

    void Cooldown() {
        timer -= Time.deltaTime;
        if (timer <= 0 && cooling && attackMode) {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack() {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    void RaycastDebugger() {
        if (distance > attackDistance) {
            Debug.DrawRay(raycast.position, vector * rayCastLength, Color.red);
        } else if (attackDistance > distance) {
            Debug.DrawRay(raycast.position, vector * rayCastLength, Color.green);
        }
    }

    public void TriggerCooling() {
        cooling = true;
    }

    private bool InsideOfLimits() {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }
    

    private void SelectTarget() {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight) {
            target = leftLimit;
        } else {
            target = rightLimit;
        }

        Flip();
    }

    private void Flip() {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x) {
            rotation.y = 0; 
            movingLeft = true;
        } else {
            rotation.y = -180;
            movingLeft = false;
        }

        transform.eulerAngles = rotation;
    }
}
