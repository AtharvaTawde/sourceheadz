using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public TextMeshProUGUI lives;
    public TextMeshProUGUI healthpots;
    public TextMeshProUGUI teller;
    public TextMeshProUGUI keyMsg;
    public GameObject healthpotImage;
    public GameObject keyImage;

    private int scene;

    void Start() {
        scene = SceneManager.GetActiveScene().buildIndex; 
        healthpotImage = GameObject.Find("UI/Health Pot Image");
        keyImage = GameObject.Find("UI/Key Image");
    }

    void Update()
    {
        GameObject player = GameObject.Find("Player");
        PlayerCombat p = player.GetComponent<PlayerCombat>();
        int currentLives = p.currentHealth;
        int maxLives = p.maxHealth;
        int healthPotCount = p.HPOTS;
        bool key = p.hasKey;
        bool opened = p.opened;

        lives.text = currentLives.ToString();

        if (healthPotCount > 1) {
            healthpots.text = healthPotCount.ToString();
        } else {
            healthpots.text = "";
        }

        if (healthPotCount > 0) {
            healthpotImage.SetActive(true);
            teller.text = "Press E to use.";
        } else {
            healthpotImage.SetActive(false);
            teller.text = "";
        }

        if (key) {
            keyImage.SetActive(true);
        } else {
            keyImage.SetActive(false);
        }

        if ((!key || !opened) && scene == 1) {
            keyMsg.text = "All you have to do is find the key. It is that simple.";
        } else if ((!key || !opened) && scene == 3) {
            keyMsg.text = "Basically just do the same thing you did last time.";
        } else if ((!key || !opened) && scene == 5) {
            keyMsg.text = "Will you even get to the key this time?";
        } else if ((!key || !opened) && scene == 8) {
            keyMsg.text = "dsviv gsv FLIP zn R?!";
        } else if ((!key || !opened) && scene == 10) {
            keyMsg.text = "Find HIM.";
        } else if (key || opened) {
            keyMsg.text = "You have ACQUIRED the KEY. You must now locate the Gateway.";
        }
    }
}
