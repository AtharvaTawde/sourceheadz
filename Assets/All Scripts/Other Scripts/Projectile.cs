using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] GameObject boulderCrumble;
    float posX;
    float boulderCrumbleLength; 
    bool createCrumble = true;

    private void Start() {
        if (name.Contains("Boulder")) {
            boulderCrumbleLength = boulderCrumble.GetComponent<ParticleSystem>().main.duration;
        }
    }

    private void Update() {
        if (gameObject.activeInHierarchy) {
            posX = transform.position.x;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject shot = other.gameObject;
        
        if (gameObject.name.Contains("Archer")) {
            SetupArcherProjectile(shot);                           
        } else if (gameObject.name.Contains("Revenant")) {
            SetupRevenantProjectile(shot);
        } else if (gameObject.name.Contains("Fireball")) {
            SetupFireball(shot);
        } else if (gameObject.name.Contains("Boulder")) {
            SetupBoulder(shot);
        } else if (gameObject.name.Contains("Triangle Bolt")) {
            SetupStratumBolt(shot);
        }
    }

    void SetupArcherProjectile(GameObject target) {
        if (target.tag == "Player") {
            target.GetComponent<PlayerCombat>().TakeDamage(22);
        } else if (target.tag == "Enemy") {
            string[] enemyNames = {"Nyert", "Goblin", "Zombi", "Archer", "Fallen", "Chicken", "Traveler"};
            foreach (string n in enemyNames) {
                if (target.name.Contains(n)) {
                    target.GetComponent<TakeDamage>().ReceiveDamage(22);
                }
            }
        }

        if (target.tag != "Water" && target.tag != "NT") {
            Destroy(gameObject);
        }
    }

    void SetupRevenantProjectile(GameObject target) {
        if (target.tag == "Player" && !target.GetComponent<PlayerMovement>().isDead) {
            target.GetComponent<PlayerCombat>().TakeDamage(15);
        } else if (target.tag == "Enemy") {
            string[] enemyNames = {"Nyert", "Goblin", "Zombi", "Archer", "Fallen", "Chicken", "Traveler"};
            foreach (string n in enemyNames) {
                if (target.name.Contains(n)) {
                    target.GetComponent<TakeDamage>().ReceiveDamage(8);
                }
            }
        }

        float power = 1500f;
        if (target.GetComponent<Rigidbody2D>() != null) {
            if (target.transform.position.x < posX) {
                target.GetComponent<Rigidbody2D>().AddForce(Vector2.left * power);
            } else {
                target.GetComponent<Rigidbody2D>().AddForce(Vector2.right * power);
            }
        }

        if (target.tag != "Water" && target.tag != "NT" && !target.name.Contains("Revenant")) {
            Destroy(gameObject);
        }
    }

    void SetupFireball(GameObject target) {
        if (target.tag == "Player" && !gameObject.name.Contains("Player")) {

            target.GetComponent<PlayerCombat>().TakeDamage(8);
            if (target.GetComponent<PlayerCombat>().burnTime == 0) {
                target.GetComponent<PlayerCombat>().burnTime = Random.Range(5f, 25f);
            } else {
                target.GetComponent<PlayerCombat>().burnTime += Random.Range(1f, 5f);
            }

        } else if (target.tag == "Enemy") {

            string[] enemyNames = {"Nyert", "Goblin", "Archer", "Revenant", "Buff", "Chicken", "Fallen", "Zombie", "Buff", "Traveler"};
            foreach (string n in enemyNames) {
                if (target.name.Contains(n)) {
                    target.GetComponent<TakeDamage>().ReceiveDamage(50);
                    if (target.GetComponent<TakeDamage>().BurnTime() > 0) {
                        target.GetComponent<TakeDamage>().SetBurnTime(Random.Range(10f, 25f));
                    } else {
                        target.GetComponent<TakeDamage>().AddBurnTime(Random.Range(5f, 10f));
                    }
                } 
            }

        } else if (target.tag == "Boss") {

            target.GetComponent<TakeDamage>().ReceiveDamage(350);

        } else if (target.tag == "Item") {
            
            Destroy(target);
        
        }

        if (target.tag != "NT" && !target.name.Contains("Gunflower") && !target.name.Contains("Rock")) {
            Destroy(gameObject);
        }
    }

    void SetupBoulder(GameObject target) {
        if (target.tag == "Player") {
            target.GetComponent<PlayerCombat>().TakeDamage(250);
        } else if (target.tag == "Enemy") {
            string[] enemyNames = {"Nyert", "Goblin", "Archer", "Fallen", "Revenant", "Buff", "Chicken", "Traveler"};
            foreach (string n in enemyNames) {
                if (target.name.Contains(n)) {
                    target.GetComponent<TakeDamage>().ReceiveDamage(250);
                } 
            }
        }

        if (target.tag != "NT" && !target.name.Contains("Gunflower") && !target.name.Contains("Rock") && !target.name.Contains("Stratum") && !target.name.Contains("Boulder")) {
            if (createCrumble) {
                GameObject boulderCrumbleInstance = Instantiate(boulderCrumble, transform.position, transform.rotation);
                Destroy(boulderCrumbleInstance, boulderCrumbleLength);
                createCrumble = false;
            }
            
            Destroy(gameObject, boulderCrumbleLength);
        }
    }

    void SetupStratumBolt(GameObject target) {
        if (target.tag == "Player") {
            target.GetComponent<PlayerCombat>().TakeDamage(300);
        } else if (target.tag == "Enemy") {
            string[] enemyNames = {"Nyert", "Goblin", "Archer", "Fallen", "Revenant", "Buff", "Chicken", "Traveler"};
            foreach (string n in enemyNames) {
                if (target.name.Contains(n)) {
                    target.GetComponent<TakeDamage>().ReceiveDamage(300);
                } 
            }
        }

        if (target.tag != "NT" && !target.name.Contains("Gunflower") && !target.name.Contains("Rock") && !target.name.Contains("Stratum") && !target.name.Contains("Boulder")) {
            Destroy(gameObject);
        }

        if (target.tag == "Block") {
            target.GetComponent<TakeDamage>().ReceiveDamage(50);
        }
    }

}
