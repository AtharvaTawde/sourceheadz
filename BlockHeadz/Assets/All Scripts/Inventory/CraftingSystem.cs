using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSystem : MonoBehaviour {
    
    [SerializeField] Item productItem;
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject[] craftingWindowObjects;
    [SerializeField] GameObject[] traders;
    [SerializeField] Image craftButtonImage;
    
    private List<string> names = new List<string>();
    private Vector3 theScale;

    public ItemSlot[] cs = new ItemSlot[4];
    public bool isCraftingMenuActive;

    private void Start() {
        inventory = GetComponent<Inventory>();
        craftingWindowObjects = GameObject.FindGameObjectsWithTag("Crafting Window");
        traders = GameObject.FindGameObjectsWithTag("Trader");

        for (int i = 0; i < craftingWindowObjects.Length; i++) {
            craftingWindowObjects[i].transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void Update() {
        theScale = craftingWindowObjects[0].transform.localScale;

        if (Input.GetKeyDown(KeyCode.C) && !IsTradingMenuActive()) {
            ToggleCraftingMenu();
        }
    }

    private void UpdateNames() {
        // automatically update names List based on what is in the crafting slots
        names.Clear();
        
        for (int j = 0; j < 4; j++) {
            names.Add("");
        }

        for (int i = 0; i < 4; i++) {
            if (cs[i].Item != null) {
                names[i] = cs[i].Item.ItemName;
            } else if (cs[i].Item == null) {
                names[i] = "";
            }
        }  
    }    

    public void Craft() {
        UpdateNames();
        List<int> amountsList = new List<int>();
        for (int i = 0; i < cs.Length; i++) {amountsList.Add(cs[i].Amount);}
        amountsList.Sort();
        int maximumTimesCraftable = amountsList[amountsList.Count - 1];

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            for (int i = 0; i < maximumTimesCraftable; i++) {
                CraftItem();
            }
        } else {
            CraftItem();
        }
    }

    void CraftItem() {
        UpdateNames();
        CorpuscleRecipe();          // Recipe 1
        BloodPieRecipe();           // Recipe 2
        BatonRecipe();              // Recipe 3
        ClayIdolRecipe();           // Recipe 4
        GlassShardRecipe();         // Recipe 5
        HealthPotRecipe();          // Recipe 6 
        StickRecipe();              // Recipe 7
        PopperRecipe();             // Recipe 8
        FabricRecipe();             // Recipe 9
        EmptyBottleRecipe();        // Recipe 10        
        StrippedLogRecipe();        // Recipe 11        
        Stick2Recipe();             // Recipe 12      
        BossGemRecipe();            // Recipe 13     
        StoneBlockRecipe();         // Recipe 14
        OvenRecipe();               // Recipe 15
        TitaniumBarRecipe();        // Recipe 16
        GoldBarRecipe();            // Recipe 17

        # region Armor    
        StoneChestplateRecipe();
        StoneLeggingsRecipe();
        StoneBootsRecipe();

        TitaniumChestplateRecipe();
        TitaniumLeggingsRecipe();
        TitaniumBootsRecipe();

        DiamondChestplateRecipe();
        DiamondLeggingsRecipe();
        DiamondBootsRecipe();

        VariumChestplateRecipe();
        VariumLeggingsRecipe();
        VariumBootsRecipe();
        # endregion

        # region Weapons
        StoneSword();
        TitaniumSword();
        DiamondSword();
        VariumSword();
        # endregion
    }
 
    // ability to add multiple amounts of a single item per crafting session
    void AddItem(string itemName, int amount) {
        bool crafted = false;
        for (int i = 0; i < amount; i++) {
            productItem = (Resources.Load("Items/" + itemName) as Item);
            if (inventory.AddItem(productItem)) {
                crafted = true;
            }
        }
        
        if (crafted) {
            StartCoroutine(ColorChange(Color.green));
            for (int k = 0; k < cs.Length; k++) { 
                cs[k].Amount--;
            }
        }
    }

    public void ToggleCraftingMenu() {
        for (int i = 0; i < craftingWindowObjects.Length; i++) {
            if (theScale == new Vector3(0, 0, 0)) {
                StartCoroutine(LerpCraftingMenu(theScale, new Vector3(1, 1, 1), craftingWindowObjects[i]));
                isCraftingMenuActive = true;
            } else if (theScale == new Vector3(1, 1, 1)) {
                StartCoroutine(LerpCraftingMenu(theScale, new Vector3(0, 0, 0), craftingWindowObjects[i]));
                isCraftingMenuActive = false;
            }
        }
    }

    public bool IsTradingMenuActive() {
        foreach (GameObject trader in traders) {
            if (trader.GetComponent<Trader>().isTradingMenuActive) {
                return true;
            }
        }
        return false;
    }

    # region Crafting Recipes

    void BloodPieRecipe() {
        SetupCrafting(new string[] {"Corpuscle", "Corpuscle", "Bread Scrap", "Bread Scrap"}, "Blood Pie", 1);
    }

    void CorpuscleRecipe() {
        SetupCrafting(new string[] {"Stone Block", "Stone Block", "Health Pot", "Diamond"}, "Corpuscle", 3);
    }

    void StickRecipe() {
        SetupCrafting(new string[] {"Wood Shard", "Wood Shard", "", ""}, "Stick", 1);
    }

    void BatonRecipe() {
        SetupCrafting(new string[] {"Diamond", "Stick", "", ""}, "Baton", 1);
    }

    void ClayIdolRecipe() {
        SetupCrafting(new string[] {"Clay Slab", "Clay Slab", "Clay Slab", "Clay Slab"}, "Clay Idol", 2);
    }

    void GlassShardRecipe() {
        SetupCrafting(new string[] {"Health Pot", "", "", ""}, "Glass Shard", 4);
    }

    void HealthPotRecipe() {
        SetupCrafting(new string[] {"Water Bottle", "Apple", "", ""}, "Health Pot", 1);
    }
    
    void FabricRecipe() {
        SetupCrafting(new string[] {"Vest", "", "", ""}, "Fabric", 4);
    }
    
    void PopperRecipe() {
        SetupCrafting(new string[] {"Fabric", "Fabric", "Gunflower Node", "Gunflower Node"}, "Popper", 1);
    }

    void EmptyBottleRecipe() {
        SetupCrafting(new string[] {"Glass Shard", "Glass Shard", "Glass Shard", "Glass Shard"}, "Empty Bottle", 1);
    }

    void StrippedLogRecipe() {
        SetupCrafting(new string[] {"Tree Trunk", "", "", ""}, "Stripped Log", 3);
    }

    void StoneBlockRecipe() {
        SetupCrafting(new string[] {"Stone", "Stone", "Stone", "Stone"}, "Stone Block", 1);
    }

    void Stick2Recipe() {
        SetupCrafting(new string[] {"Stripped Log", "Stripped Log", "", ""}, "Stick", 4);
    }

    void BossGemRecipe() {
        SetupCrafting(new string[] {"Red Crystal", "Blue Crystal", "Yellow Crystal", "Green Crystal"}, "Boss Gem", 1);
    }

    void OvenRecipe() {
        SetupCrafting(new string[] {"Titanium Bar", "Titanium Bar", "Stone Block", "Stone Block"}, "Oven", 1);
    }

    void TitaniumBarRecipe() {
        SetupCrafting(new string[] {"Titanium Nugget", "Titanium Nugget", "Titanium Nugget", "Titanium Nugget"}, "Titanium Bar", 1);
    }

    void GoldBarRecipe() {
        SetupCrafting(new string[] {"Gold Scrap", "Gold Scrap", "Gold Scrap", "Gold Scrap"}, "Gold Bar", 1);
    }

    # region Armor
    void StoneChestplateRecipe() {
        SetupCrafting(new string[] {"Stone Block", "Stone Block", "Stone Block", "Fabric"}, "Stone Chestplate", 1);
    }

    void StoneLeggingsRecipe() {
        SetupCrafting(new string[] {"Stone Block", "Stone Block", "Fabric", "Fabric"}, "Stone Leggings", 1);
    }

    void StoneBootsRecipe() {
        SetupCrafting(new string[] {"Stone Block", "Fabric", "Fabric", "Fabric"}, "Stone Boots", 1);
    }

    void TitaniumChestplateRecipe() {
        SetupCrafting(new string[] {"Titanium Bar", "Titanium Bar", "Titanium Bar", "Fabric"}, "Titanium Chestplate", 1);
    }

    void TitaniumLeggingsRecipe() {
        SetupCrafting(new string[] {"Titanium Bar", "Titanium Bar", "Fabric", "Fabric"}, "Titanium Leggings", 1);
    }

    void TitaniumBootsRecipe() {
        SetupCrafting(new string[] {"Titanium Bar", "Fabric", "Fabric", "Fabric"}, "Titanium Boots", 1);
    }

    void DiamondChestplateRecipe() {
        SetupCrafting(new string[] {"Diamond", "Diamond", "Diamond", "Fabric"}, "Diamond Chestplate", 1);
    }

    void DiamondLeggingsRecipe() {
        SetupCrafting(new string[] {"Diamond", "Diamond", "Fabric", "Fabric"}, "Diamond Leggings", 1);
    }

    void DiamondBootsRecipe() {
        SetupCrafting(new string[] {"Diamond", "Fabric", "Fabric", "Fabric"}, "Diamond Boots", 1);
    }

    void VariumChestplateRecipe() {
        SetupCrafting(new string[] {"Varium", "Varium", "Varium", "Fabric"}, "Varium Chestplate", 1);
    }

    void VariumLeggingsRecipe() {
        SetupCrafting(new string[] {"Varium", "Varium", "Fabric", "Fabric"}, "Varium Leggings", 1);
    }

    void VariumBootsRecipe() {
        SetupCrafting(new string[] {"Varium", "Fabric", "Fabric", "Fabric"}, "Varium Boots", 1);
    }
    # endregion
    
    # region Weapons

    void StoneSword() {
        SetupCrafting(new string[] {"Stone Block", "Stone Block", "Stick", ""}, "Stone Sword", 1);

    }
    void TitaniumSword() {
        SetupCrafting(new string[] {"Titanium Bar", "Titanium Bar", "Stick", ""}, "Titanium Sword", 1);
    }

    void DiamondSword() {
        SetupCrafting(new string[] {"Diamond", "Diamond", "Stick", ""}, "Diamond Sword", 1);
    }

    void VariumSword() {
        SetupCrafting(new string[] {"Varium", "Varium", "Stick", ""}, "Varium Sword", 1);
    }

    # endregion

    # endregion

    void SetupCrafting(string[] ingredientsInit, string product, int amount) {
        List<string> ingredients = new List<string>();
        List<string> checks = new List<string>();
        
        for (int i = 0; i < ingredientsInit.Length; i++) {
            ingredients.Add(ingredientsInit[i]);
        }

        for (int j = 0; j < ingredientsInit.Length; j++) {   
            if (!names.Contains(ingredients[0])) {
                checks.Add("");
                names.Remove(names[0]);
                ingredients.Remove(ingredients[0]);
            } else {
                names.Remove(ingredients[0]);
                ingredients.Remove(ingredients[0]);
            }
        }

        if (checks.Count == 0) {
            AddItem(product, amount);  
        }

        UpdateNames();
    }

    IEnumerator ColorChange(Color color) {
        float elapsedTime = 0f;
        float waitTime = 0.5f;

        while (elapsedTime < waitTime) {
            craftButtonImage.color = Color.Lerp(color, Color.white, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        craftButtonImage.color = Color.white;
        yield return null;
    }

    IEnumerator LerpCraftingMenu(Vector3 startScale, Vector3 finalScale, GameObject thing) {
        float elapsedTime = 0f;
        float waitTime = 0.05f;

        while (elapsedTime < waitTime) {
            thing.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.transform.localScale = finalScale;
        yield return null;
    }
}