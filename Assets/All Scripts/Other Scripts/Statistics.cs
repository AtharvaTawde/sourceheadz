using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Statistics : MonoBehaviour {
    
    [SerializeField] GameObject[] containers = new GameObject[12];
    private TextMeshProUGUI[] names = new TextMeshProUGUI[12];
    private Image[] images = new Image[12];
    private TextMeshProUGUI[] amounts = new TextMeshProUGUI[12];

    private string[] mobNames = {
        "Nyerts",
        "Goblins",
        "Fallens",
        "Archers",
        "Zombies",
        "Revenants",
        "Gunflowers",
        "Gunflowers",
        "Firetraps",
        "Chicken",
        "Baby Chicken",
        "Stratum"
    };

    private Dictionary<string, string> stats = new Dictionary<string, string> {
        //   Statistic               Action
        {"Nyerts Killed",           "Killed"},
        {"Goblins Killed",          "Killed"},
        {"Fallens Killed",          "Killed"},
        {"Archers Killed",          "Killed"},
        {"Zombies Killed",          "Killed"},
        {"Revenants Killed",        "Killed"},
        {"Gunflowers Planted",      "Planted"},
        {"Gunflowers Harvested",    "Harvested"},
        {"Firetraps Killed",        "Killed"},
        {"Chickens Killed",         "Killed"},
        {"Baby Chickens Killed",    "Killed"},
        {"Bosses Killed",           "Killed"},
        //{"Buffs Killed",            "Killed"}
    };

    private void Start() {
        for (int i = 0; i < containers.Length; i++) {
            containers[i] = GameObject.Find(string.Format("Main Menu/Scroll/Panel/Container ({0})", i));
            names[i] = containers[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            images[i] = containers[i].transform.GetChild(1).GetComponent<Image>();
            amounts[i] = containers[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        }

        for (int i = 0; i < amounts.Length; i++) {
            string action = stats.Values.ToArray()[i];
            int value = PlayerPrefs.GetInt(stats.Keys.ToArray()[i]);

            amounts[i].text = string.Format("{0}: {1}", action, value);

            if (value == 0) {
                names[i].text = "???";
                images[i].color = Color.black;
            } else {
                names[i].text = mobNames[i];
                images[i].color = Color.white;
            }
        }
    }

}
