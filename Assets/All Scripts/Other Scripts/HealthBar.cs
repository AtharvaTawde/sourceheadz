using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private PlayerCombat player;
    private Stratum stratum;
    private ItemSelection itemSelection;

    private void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerCombat>();
        itemSelection = GameObject.Find("Player").GetComponent<ItemSelection>();
        stratum = GameObject.Find("Stratum").GetComponent<Stratum>();
    }

    void Update() {
        if (!name.Contains("Stratum")) {
            PlayerDisplay();
        } else if (name.Contains("Stratum")) {
            StratumDisplay();
        }
    }

    void PlayerDisplay() {
        if (name.Contains("Health")) {
            if (player != null) {
                slider.maxValue = player.maxHealth;

                if (!player.GetComponent<PlayerMovement>().isDead) {
                    slider.value = player.currentHealth;
                } else { 
                    slider.value = 0;
                }

                fill.color = gradient.Evaluate(slider.normalizedValue);
            }
        } else if (name.Contains("Hunger")) {
            if (player != null) {
                slider.maxValue = player.maxHunger;

                if (!player.GetComponent<PlayerMovement>().isDead) {
                    slider.value = player.displayHunger;
                } else { 
                    slider.value = 0;
                }

            }
        } else if (name.Contains("Eating")) {

            if (player != null) {
                slider.maxValue = itemSelection.k_eatTime;

                if (!player.GetComponent<PlayerMovement>().isDead) {
                    slider.value = itemSelection.eatTime;
                }

            }
        }
    }

    void StratumDisplay() {
        if (name.Contains("Health")) {
            if (stratum != null) {
                slider.maxValue = stratum.maxHealth;
                slider.value = stratum.currentHealth;
            }
        }
    }

}
