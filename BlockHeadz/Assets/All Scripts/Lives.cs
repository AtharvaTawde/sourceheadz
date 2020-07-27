using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Lives : MonoBehaviour
{
    public TextMeshProUGUI lives;
    public TextMeshProUGUI healthpots;
    public TextMeshProUGUI teller;
    public TextMeshProUGUI keyMsg;

    private int scene;

    void Start() {
        scene = SceneManager.GetActiveScene().buildIndex; 
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

        if (currentLives > 0) {
            lives.text = currentLives.ToString();
        }

        if (healthPotCount > 0) {
            healthpots.text = "Health Pots Obtained: " + healthPotCount.ToString();
            teller.text = "Press E to drink (+25 Health). Warning: You cannot increase your maximum health this way.";
        } else {
            healthpots.text = "";
            teller.text = "";
        }
        
        Debug.Log(scene);

        if ((!key || !opened) && scene == 1) {
            keyMsg.text = "All you have to do is find the key. It is that simple.";
        } else if ((!key || !opened) && scene == 3) {
            keyMsg.text = "Basically just do the same thing you did last time.";
        } else if ((!key || !opened) && scene == 5) {
            keyMsg.text = "Will you even get to the key this time?";
        } else if ((!key || !opened) && scene == 8) {
            keyMsg.text = "dsviv gsv FLIP zn R?!";
        } else if (key || opened) {
            keyMsg.text = "You have ACQUIRED the KEY. You must now locate the Gateway.";
        }
    }
}
