using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Statistics : MonoBehaviour {
    
    public TextMeshProUGUI goblinsKilled, zombiesKilled, nyertsKilled, archersKilled;

    private void Update() {
        goblinsKilled.text = "Goblins Killed: " + PlayerPrefs.GetInt("Goblins Killed");
        zombiesKilled.text = "Zombies Killed: " + PlayerPrefs.GetInt("Zombies Killed");
        nyertsKilled.text = "Nyerts Killed: " + PlayerPrefs.GetInt("Nyerts Killed");
        archersKilled.text = "Archers Killed: " + PlayerPrefs.GetInt("Archers Killed"); 
    }

}
