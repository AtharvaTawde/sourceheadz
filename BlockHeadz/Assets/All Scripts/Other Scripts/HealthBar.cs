using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    void Update() {
        GameObject player = GameObject.Find("Player");

        if (player != null) {
            PlayerCombat p = player.GetComponent<PlayerCombat>();
            slider.maxValue = p.maxHealth;

            if (!player.GetComponent<PlayerCombat>().dead) {
                slider.value = p.currentHealth;
            } else { 
                slider.value = 0;
            }

            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
