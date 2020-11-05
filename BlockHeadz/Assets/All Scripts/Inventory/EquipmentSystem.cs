using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour {
 
    public ItemSlot[] equipmentSlots = new ItemSlot[4];
    private List<string> equipmentNames = new List<string>();
    string chestplateName, leggingsName, bootsName, weaponName;
    private string stoneColor = "#6f6f6f";
    private string variumColor = "#2fa07b";
    private string diamondColor = "#2f90d4";
    private string tungstenCarbideColor = "#a45bb2";
    private string titaniumColor = "#979f8f";
    public List<Color> typeColors = new List<Color>();
    
    private string[] chestplateType = {"Stone Chestplate", 
                                       "Tungsten Carbide Chestplate", 
                                       "Titanium Chestplate",
                                       "Diamond Chestplate", 
                                       "Varium Chestplate"}; 

    private string[] leggingsType = {"Stone Leggings", 
                                     "Tungsten Carbide Leggings", 
                                     "Titanium Leggings",
                                     "Diamond Leggings", 
                                     "Varium Leggings"};

    private string[] bootsType = {"Stone Boots", 
                                  "Tungsten Carbide Boots", 
                                  "Titanium Boots",
                                  "Diamond Boots", 
                                  "Varium Boots"};

    private string[] swordType = {"Stone Sword", 
                                  "Tungsten Carbide Sword", 
                                  "Titanium Sword",
                                  "Diamond Sword", 
                                  "Varium Sword"};

    private string[] knifeType = {"Stone Knife", 
                                  "Tungsten Carbide Knife", 
                                  "Titanium Knife",
                                  "Diamond Knife", 
                                  "Varium Knife"};

    private string[] axeType = {"Stone Axe", 
                                "Tungsten Carbide Axe", 
                                "Titanium Axe",
                                "Diamond Axe", 
                                "Varium Axe"};

    public GameObject chestplate;
    public GameObject leggings;
    public GameObject boots;
    public GameObject sword;

    float[] chestplateProtValues = {0.07f, 0.15f, 0.25f, 0.35f, 0.45f};
    float[] leggingsProtValues = {0.05f, 0.1f, 0.16f, 0.23f, 0.3f};
    float[] bootsProtValues = {0.03f, 0.05f, 0.09f, 0.12f, 0.15f};
    int[] swordAtkValues = {40, 105, 135, 165, 210};
    
    public float damageReduction;
    float chestplateReduction, bootsReduction, leggingsReduction;

    public int weaponDamageIncrease;
    
    

    void Unhexify(string hexColor) {
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
            typeColors.Add(color);
    }

    void InitColors() {
        Unhexify(stoneColor);
        Unhexify(tungstenCarbideColor);
        Unhexify(titaniumColor);
        Unhexify(diamondColor);
        Unhexify(variumColor);
    }

    private void Start() {
        InitColors();
        for (int i = 0; i < 4; i++) {equipmentNames.Add("");}
        chestplate.SetActive(true);
        leggings.SetActive(true);
        boots.SetActive(true);
        sword.SetActive(true);
    }

    private void Update() {
        SetupSlots();
        ChestplateManager();
        LeggingsManager();
        BootsManager();
        SwordManager();

        damageReduction = chestplateReduction + leggingsReduction + bootsReduction;
    }

    void ChestplateManager() {
        for (int i = 0; i < chestplateType.Length; i++) {
            if (chestplateName.Contains(chestplateType[i]) && chestplate != null) {
                chestplate.GetComponent<SpriteRenderer>().color = typeColors[i];
                chestplateReduction = chestplateProtValues[i];
            } else if (chestplateName == "" && chestplate != null) {
                chestplate.GetComponent<SpriteRenderer>().color = Color.clear;
                chestplateReduction = 0f;
            }
        } 
    }

    void LeggingsManager() {
        for (int i = 0; i < leggingsType.Length; i++) {
            if (leggingsName.Contains(leggingsType[i]) && leggings != null) {
                leggings.GetComponent<SpriteRenderer>().color = typeColors[i];
                leggingsReduction = leggingsProtValues[i];
            } else if (leggingsName == "" && leggings != null) {
                leggings.GetComponent<SpriteRenderer>().color = Color.clear;
                leggingsReduction = 0f;
            }
        }
    }

    void BootsManager() {
        for (int i = 0; i < bootsType.Length; i++) {
            if (bootsName.Contains(bootsType[i]) && boots != null) {
                boots.GetComponent<SpriteRenderer>().color = typeColors[i];
                bootsReduction = bootsProtValues[i];
            } else if (bootsName == "" && boots != null) {
                boots.GetComponent<SpriteRenderer>().color = Color.clear;
                bootsReduction = 0f;
            }
        }
    }

    void SwordManager() {
        for (int i = 0; i < swordType.Length; i++) {
            if (weaponName.Contains(swordType[i]) && sword != null) {
                sword.GetComponent<SpriteRenderer>().color = typeColors[i];
                weaponDamageIncrease = swordAtkValues[i];
            } else if (weaponName == "" && sword != null) {
                sword.GetComponent<SpriteRenderer>().color = Color.clear;
                weaponDamageIncrease = 0;
            }
        }
    }

    void SetupSlots() {
        for (int i = 0; i < equipmentNames.Count; i++) {
            if (equipmentSlots[i].Item != null) {
                equipmentNames[i] = equipmentSlots[i].Item.ItemName;
            } else {
                equipmentNames[i] = "";
            }                 
        }

        chestplateName = equipmentNames[0];
        leggingsName = equipmentNames[1];
        bootsName = equipmentNames[2];
        weaponName = equipmentNames[3];
    }

}
