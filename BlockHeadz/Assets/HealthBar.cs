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
        PlayerCombat p = player.GetComponent<PlayerCombat>();
        slider.maxValue = p.maxHealth;
        slider.value = p.currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
