using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockIndicator : MonoBehaviour {
    

    public string n;
    public float blockBreakTime;
    [SerializeField] GameObject crack;
    public Animator crackAnimator;

    private PlayerCombat player;
    private GameObject hoveredGO;
    private int playerDamage;
    private float officialBreakTime;
    private float[] blockHealth = {100f, 100f, 150f, 350f, 150f, 75f, 150f};
    private string[] names = {"Tree Trunk", "Stripped Log", "Stone Block", "Oven", "Chest", "Leaves", "Apples"};

    private void Start() {
        if (gameObject.activeInHierarchy) {
            player = GameObject.Find("Player").GetComponent<PlayerCombat>();
        }
        n = gameObject.name;
        crack = transform.GetChild(0).gameObject;
        crackAnimator.GetComponent<Animator>();
    }    
    
    private void OnValidate() {
        if (gameObject.activeInHierarchy) {
            player = GameObject.Find("Player").GetComponent<PlayerCombat>();
        }

        n = gameObject.name;
        crack = transform.GetChild(0).gameObject;
        crackAnimator = crack.GetComponent<Animator>();
    }    

    private void Update() {
        playerDamage = player.atkdmg;
        officialBreakTime = player.breakTime;
        if (player.hit.collider != null) 
            hoveredGO = player.hit.collider.gameObject;

        for (int i = 0; i < names.Length; i++) {
            if (n.Contains(names[i])) {
                blockBreakTime = blockHealth[i] / playerDamage;
                break;
            }
        }
    }

}
