using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Microwave : MonoBehaviour {

    PlayerCombat playerCombat;
    public bool microwaveActivated = false;
    GameObject microwaveUI;

    GameObject[] slots = new GameObject[4];
    GameObject productSlot;

    private void Start() {
        playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();
        //microwaveUI = GameObject.Find("UI/Microwave Interface");
        //microwaveUI.SetActive(false);

        //for (int i = 0; i < slots.Length; i++) {
        //    slots[i] = microwaveUI.transform.Find("Slot " + i).gameObject;
        //}
        //productSlot = microwaveUI.transform.Find("Product Slot").gameObject;

    }

    public void Activate() {
        microwaveUI.SetActive(true);
        microwaveActivated = true;
    }

    public void Deactivate() {
        microwaveUI.SetActive(false);
        microwaveActivated = false;
    }


}
