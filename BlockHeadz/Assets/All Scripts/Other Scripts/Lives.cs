using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lives : MonoBehaviour {
    
    # region Stuff
    public TextMeshProUGUI lives;
    public TextMeshProUGUI keyMsg;
    public TextMeshProUGUI protLvl;
    public TextMeshProUGUI atkDmg;
    public GameObject protImage;
    public GameObject keyImage;
    [SerializeField] GameObject inventory;
    [SerializeField] float damageReduction;
    [SerializeField] int attackDamage;
    PlayerCombat playerCombat;
    private int scene;
    # endregion

    void Start() {
       playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();
    } 

    void Update() {
        damageReduction = inventory.GetComponent<EquipmentSystem>().damageReduction;
        attackDamage = playerCombat.atkdmg;
        int currentLives = playerCombat.currentHealth;
        int maxLives = playerCombat.maxHealth;
        
        protLvl.text = Mathf.RoundToInt(damageReduction * 100f).ToString() + "%";
        atkDmg.text = attackDamage.ToString();

        if (damageReduction != 0f) {
            protImage.SetActive(true);
        } else {
            protImage.SetActive(false);
        }

        if (playerCombat != null) {
            lives.text = currentLives.ToString();
            SetKey();
        }
    }

    void SetKey() {
        bool key = playerCombat.hasKey;
        bool opened = playerCombat.opened;
        if (key) {
            keyImage.SetActive(true);
        } else {
            keyImage.SetActive(false);
        }

        if ((!key || !opened)) {
            keyMsg.text = "Find the Key.";
        } else if (key || opened) {
            keyMsg.text = "Locate the Gateway.";
        }
    }
}