using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour {
 
    public ItemSlot[] equipmentSlots = new ItemSlot[4];
    public List<string> equipmentNames = new List<string>();
    string chestplateName, leggingsName, bootsName, weaponName;
    private string stoneColor = "#6f6f6f";
    private string variumColor = "#2fa07b";
    private string diamondColor = "#2f90d4";
    private string tungstenCarbideColor = "#a45bb2";
    private string titaniumColor = "#f1faea";
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

    public GameObject chestplate;
    public GameObject leggings;
    public GameObject boots;
    public GameObject sword;

    float[] chestplateProtValues =  {0.12f, 0.18f, 0.27f, 0.36f, 0.45f};
    float[] leggingsProtValues =    {0.08f, 0.12f, 0.18f, 0.24f, 0.30f};
    float[] bootsProtValues =       {0.04f, 0.06f, 0.09f, 0.12f, 0.15f};
    // Protection Values              24%    35%    54%    72%    90%  

    int[] swordAtkValues = {40, 75, 105, 135, 200};

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
            if (chestplateName == chestplateType[i]) {
                chestplate.GetComponent<SpriteRenderer>().color = typeColors[i];
                chestplateReduction = chestplateProtValues[i];
            } else if (!chestplateName.Contains("Chestplate")) {
                chestplate.GetComponent<SpriteRenderer>().color = Color.clear;
                chestplateReduction = 0f;
            }
        } 
        SetupSlots();
    }

    void LeggingsManager() {
        for (int i = 0; i < leggingsType.Length; i++) {
            if (leggingsName == leggingsType[i]) {
                leggings.GetComponent<SpriteRenderer>().color = typeColors[i];
                leggingsReduction = leggingsProtValues[i];
            } else if (!leggingsName.Contains("Leggings")) {
                leggings.GetComponent<SpriteRenderer>().color = Color.clear;
                leggingsReduction = 0f;
            }
        }
        SetupSlots();
    }

    void BootsManager() {
        for (int i = 0; i < bootsType.Length; i++) {
            if (bootsName == bootsType[i]) {
                boots.GetComponent<SpriteRenderer>().color = typeColors[i];
                bootsReduction = bootsProtValues[i];
            } else if (!bootsName.Contains("Boots")) {
                boots.GetComponent<SpriteRenderer>().color = Color.clear;
                bootsReduction = 0f;
            }
        }
        SetupSlots();
    }

    void SwordManager() {
        for (int i = 0; i < swordType.Length; i++) {
            if (weaponName == swordType[i]) {
                sword.GetComponent<SpriteRenderer>().color = typeColors[i];
                weaponDamageIncrease = swordAtkValues[i];
            } else if (!weaponName.Contains("Sword")) {
                sword.GetComponent<SpriteRenderer>().color = Color.clear;
                weaponDamageIncrease = 0;
            }
        }
        SetupSlots();
    }

    void SetupSlots() {
        for (int i = 0; i < 4; i++) {
            if (equipmentSlots[i].Item != null) {
                equipmentNames[i] = equipmentSlots[i].Item.ItemName;
            } else if (equipmentSlots[i].Item == null) {
                equipmentNames[i] = "";
            }                 
        }

        chestplateName = equipmentNames[0];
        leggingsName = equipmentNames[1];
        bootsName = equipmentNames[2];
        weaponName = equipmentNames[3];
    }

}
