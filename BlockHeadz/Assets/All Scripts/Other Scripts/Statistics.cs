using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Statistics : MonoBehaviour {
    
    public TextMeshProUGUI[] texts;
    string[] strings = {"Goblins Killed", 
                        "Zombies Killed", 
                        "Nyerts Killed", 
                        "Archers Killed", 
                        "Revenants Killed", 
                        "Fallens Killed"};

    private void Start() {
        for (int i = 0; i < strings.Length; i++) {
            texts[i] = GameObject.Find("Main Menu/Panel/TextField (" + i.ToString() + ")").GetComponent<TextMeshProUGUI>(); 
        }
    }

    private void Update() {
        for (int i = 0; i < texts.Length; i++) {
            texts[i].text = strings[i] + ": " + PlayerPrefs.GetInt(strings[i]);
        }
    }

}
