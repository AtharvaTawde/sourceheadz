using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSystem : MonoBehaviour {
    
    [SerializeField] Item productItem;
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject[] craftingWindowObjects;
    [SerializeField] Image craftButtonImage;
    
    List<string> names = new List<string>();
    public ItemSlot[] cs = new ItemSlot[4];

    private void Start() {
        inventory = GetComponent<Inventory>();
        craftingWindowObjects = GameObject.FindGameObjectsWithTag("Crafting Window");

        for (int i = 0; i < craftingWindowObjects.Length; i++) {
            craftingWindowObjects[i].transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void Update() {
        
        Vector3 theScale = craftingWindowObjects[0].transform.localScale;

        if (Input.GetKeyDown(KeyCode.C)) {
            for (int i = 0; i < craftingWindowObjects.Length; i++) {
                if (theScale == new Vector3(0, 0, 0)) {
                    StartCoroutine(LerpCraftingMenu(theScale, new Vector3(1, 1, 1), craftingWindowObjects[i]));
                } else if (theScale == new Vector3(1, 1, 1)) {
                    StartCoroutine(LerpCraftingMenu(theScale, new Vector3(0, 0, 0), craftingWindowObjects[i]));
                }
            }
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
        CorpuscleRecipe();          // Recipe 1
        BloodPieRecipe();           // Recipe 2
        BatonRecipe();              // Recipe 3
        ClayIdolRecipe();           // Recipe 4
        GlassShardRecipe();         // Recipe 5
        HealthPotRecipe();          // Recipe 6 
        StickRecipe();              // Recipe 7
        PopperRecipe();             // Recipe 8
        FabricRecipe();             // Recipe 9
        
        //VestRecipe();               // Recipe 10 [REDACTED FOR NOW: MAKES NO SENSE]

        # region Armor    
        StoneChestplateRecipe();
        StoneLeggingsRecipe();
        StoneBootsRecipe();
        
        DiamondChestplateRecipe();
        DiamondLeggingsRecipe();
        DiamondBootsRecipe();
        # endregion

        # region Weapons
        StoneSword();
        DiamondSword();
        # endregion

    }

    // ability to add multiple amounts of a single item per crafting session
    void AddItem(string itemName, int amount) {
        bool crafted = false;
        for (int i = 0; i < amount; i++) {
            productItem = (Resources.Load("Items/" + itemName) as Item).GetCopy();
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

    # region Crafting Recipes

    void BloodPieRecipe() {
        SetupCrafting(new string[] {"Corpuscle", "Bread Scrap", "Health Pot", "Goblin Meat"}, "Blood Pie", 1);
    }

    void CorpuscleRecipe() {
        SetupCrafting(new string[] {"Goblin Meat", "Nyert Meat", "Health Pot", ""}, "Corpuscle", 2);
    }

    void StickRecipe() {
        SetupCrafting(new string[] {"Wood Shard", "Wood Shard", "", ""}, "Stick", 1);
    }

    void BatonRecipe() {
        SetupCrafting(new string[] {"Diamond", "Stick", "", ""}, "Baton", 1);
    }

    void ClayIdolRecipe() {
        SetupCrafting(new string[] {"Stone", "Health Pot", "Clay Slab", "Corpuscle"}, "Clay Idol", 2);
    }

    void GlassShardRecipe() {
        SetupCrafting(new string[] {"Health Pot", "", "", ""}, "Glass Shard", 4);
    }

    void HealthPotRecipe() {
        SetupCrafting(new string[] {"Corpuscle", "Glass Shard", "", ""}, "Health Pot", 1);
    }
    
    void FabricRecipe() {
        SetupCrafting(new string[] {"Vest", "", "", ""}, "Fabric", 4);
    }
    
    void PopperRecipe() {
        SetupCrafting(new string[] {"Fabric", "Fabric", "Gunflower Node", "Gunflower Node"}, "Popper", 1);
    }

    void VestRecipe() {
        SetupCrafting(new string[] {"Fabric", "Fabric", "Fabric", "Fabric"}, "Vest", 1);
    }

    # region Armor
    void StoneChestplateRecipe() {
        SetupCrafting(new string[] {"Stone", "Stone", "Stone", "Fabric"}, "StoneC", 1);
    }

    void StoneLeggingsRecipe() {
        SetupCrafting(new string[] {"Stone", "Stone", "Fabric", "Fabric"}, "StoneL", 1);
    }

    void StoneBootsRecipe() {
        SetupCrafting(new string[] {"Stone", "Fabric", "Fabric", "Fabric"}, "StoneB", 1);
    }

    void DiamondChestplateRecipe() {
        SetupCrafting(new string[] {"Diamond", "Diamond", "Diamond", "Fabric"}, "DiamondC", 1);
    }

    void DiamondLeggingsRecipe() {
        SetupCrafting(new string[] {"Diamond", "Diamond", "Fabric", "Fabric"}, "DiamondL", 1);
    }

    void DiamondBootsRecipe() {
        SetupCrafting(new string[] {"Diamond", "Fabric", "Fabric", "Fabric"}, "DiamondB", 1);
    }
    # endregion
    
    # region Weapons

    void StoneSword() {
        SetupCrafting(new string[] {"Stone", "Stone", "Stick", ""}, "StoneSW", 1);
    }

    void DiamondSword() {
        SetupCrafting(new string[] {"Diamond", "Diamond", "Stick", ""}, "DiamondSW", 1);
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