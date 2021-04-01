using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lives : MonoBehaviour {
    
    public TextMeshProUGUI lives;
    public TextMeshProUGUI protLvl;
    public TextMeshProUGUI atkDmg;
    public GameObject protImage;
    
    [SerializeField] GameObject inventory;
    [SerializeField] GameObject eatSlider;
    [SerializeField] float damageReduction;
    [SerializeField] float consumptionTime;
    [SerializeField] int attackDamage;
    
    private PlayerCombat playerCombat;
    private ItemSelection itemSelection;
    private PlayerMovement playerMovement;
    private int scene;

    void Start() {
       playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
       playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();
       itemSelection = GameObject.Find("Player").GetComponent<ItemSelection>();
    } 

    void Update() {
        damageReduction = inventory.GetComponent<EquipmentSystem>().damageReduction;
        attackDamage = playerCombat.atkdmg;
        consumptionTime = itemSelection.eatTime;
        int currentLives = playerCombat.currentHealth;
        int maxLives = playerCombat.maxHealth;
        
        protLvl.text = Mathf.RoundToInt(damageReduction * 100f).ToString() + "%";
        atkDmg.text = attackDamage.ToString();

        if (consumptionTime <= 0 || consumptionTime == itemSelection.k_eatTime) {
            eatSlider.GetComponent<CanvasGroup>().alpha = 0f;
        } else {
            eatSlider.GetComponent<CanvasGroup>().alpha = 1f;
        }

        if (playerMovement.isDead || !itemSelection.isFood()) {
            eatSlider.GetComponent<CanvasGroup>().alpha = 0f;
        }

        if (damageReduction != 0f) {
            protImage.SetActive(true);
        } else {
            protImage.SetActive(false);
        }

        if (playerCombat != null) {
            lives.text = currentLives.ToString();
        }
    }

}       