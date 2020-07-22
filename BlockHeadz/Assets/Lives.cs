using UnityEngine;
using TMPro;

public class Lives : MonoBehaviour
{
    //private int localLives;
    //private int healthPotCount;
    public TextMeshProUGUI lives;
    public TextMeshProUGUI healthpots;
    public TextMeshProUGUI teller;
    public TextMeshProUGUI keyMsg;

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

        if (key || opened) {
            keyMsg.text = "You have acquired the key. You have also opened the Gateway.";
        } else {
            keyMsg.text = "You must find the key. Not very hard...";
        }
    }
}
