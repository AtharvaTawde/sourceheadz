using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Shop : MonoBehaviour {
    
    string itemUnlocked;
    
    GameObject[] accessories = new GameObject[8]; // actual number of items + 1
    string[] names = {"None", "Pan", "Cone", "Banana", "Trophy", "Spartan Helmet", "Magical Hat", "Top Hat", ""};
    string[] descriptions = {"This is absolutely nothing. The OG fit.", 
                            "Fry eggs on it. Or anything. I don't really care.", 
                            "Stop right there.", 
                            "Flex with the ultimate Banana. Serves as a healthy snack afterwards.", 
                            "You won. You deserve this.", 
                            "Killer of all.", 
                            "Ah. I see. Now you are getting interested in magic...", 
                            "Did you have a great time at Bartie's?"};
    GameObject selectBar, selectedGameObject;
    Vector3 currentPosition, selectedPosition, targetPosition;
    public TextMeshProUGUI description;

    private void Start() {
        selectBar = GameObject.Find("Shop/SelectBar");
        selectBar.transform.position = GameObject.Find("Shop/" + PlayerPrefs.GetString("Selected Item")).transform.position;
        for (int i = 0; i < accessories.Length; i++) {
            accessories[i] = GameObject.Find("Shop/Item" + i);
            if (i != 0)
                accessories[i].GetComponent<Button>().interactable = false;
        } 

        if (PlayerPrefs.GetString("Selected Item") == "") {
            PlayerPrefs.SetString("Selected Item", "Item0");
            selectBar.transform.position = GameObject.Find("Shop/Item0").transform.position;
        }
    }
    
    private void Update() {
        for (int i = 1; i < accessories.Length; i++) {
            itemUnlocked = PlayerPrefs.GetString("Item" + i);
            if (itemUnlocked == "True") {
                accessories[i].GetComponent<Button>().interactable = true;
            }
        }    

        // Description Initializer
        for (int j = 0; j < accessories.Length; j++) {
            if (PlayerPrefs.GetString("Selected Item") == "Item" + j) {
                string name = names[j];
                string d = descriptions[j];
                description.text = name + ": " + d;
            }
        }

        //DebuggerFunction();
    }

    public void EquipAccessory() {
        currentPosition = selectBar.transform.position;
        selectedPosition = EventSystem.current.currentSelectedGameObject.transform.position;
        selectedGameObject = EventSystem.current.currentSelectedGameObject;
        targetPosition = new Vector3(selectedPosition.x, selectedPosition.y, selectedPosition.z);
        StartCoroutine(LerpPosition(targetPosition, .1f));        
        PlayerPrefs.SetString("Selected Item", selectedGameObject.name);
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration) {
        float time = 0;
        Vector3 startPosition = selectBar.transform.position;
        while (time < duration) {
            selectBar.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        selectBar.transform.position = targetPosition;
    }

    void DebuggerFunction() {
        if (Input.GetKeyDown(KeyCode.A)) {
            for (int i = 1; i < accessories.Length; i++) {
                accessories[i].GetComponent<Button>().interactable = !accessories[i].GetComponent<Button>().interactable;
            }
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            accessories[7].GetComponent<Button>().interactable = !accessories[7].GetComponent<Button>().interactable;
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            accessories[6].GetComponent<Button>().interactable = !accessories[6].GetComponent<Button>().interactable;
        }
    }
}
    