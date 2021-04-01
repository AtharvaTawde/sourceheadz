using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Random=UnityEngine.Random;
using System;
using UnityEngine.Events;

public class Trader : MonoBehaviour {
 
    [SerializeField] Sprite[] items;
    [SerializeField] GameObject[] tradingWindowObjects;
    [SerializeField] CraftingSystem c;
    [SerializeField] ItemSlot tradingSlot;  
    [SerializeField] Inventory inventory;
    [SerializeField] Button nextTradeBtn;
    [SerializeField] Button prevTradeBtn;
    [SerializeField] Button tradeBtn;

    public bool isTradingMenuActive;
    public bool hoverOverSelf;

    private Vector3 spawnPoint, realPoint;
    private string[] traderTypes = {"Forger", "Rancher", "Lumberjack"};
    private string hoveringOver;
    private int currentIndexShowing = 0; 
    private string traderType;

    [SerializeField] Sprite[] forgerItems;   

    #region Trading Items
    [SerializeField] Image reactantImage;
    [SerializeField] Image productImage;
    [SerializeField] string reactant;
    [SerializeField] string product;
    [SerializeField] int reactantAmount;
    [SerializeField] int productAmount;
    [SerializeField] TextMeshProUGUI reactantAmountText;
    [SerializeField] TextMeshProUGUI productAmountText;
    [SerializeField] TextMeshProUGUI tradeCounterText;
    private string[] r;
    private string[] p;
    #endregion

    Dictionary<string, string> allForgerTradesPossible = new Dictionary<string, string> {
        // Sell  
        {"Tungsten Carbide Boots"      + "x1" ,       "Gold Barx2"},
        {"Tungsten Carbide Leggings"   + "x1" ,       "Gold Barx3"},
        {"Tungsten Carbide Chestplate" + "x1" ,       "Gold Barx4"},
        {"Titanium Nugget"             + "x2" ,       "Gold Barx1"},
        {"Titanium Bar"                + "x1" ,       "Gold Barx2"},
        {"Titanium Boots"              + "x1" ,       "Gold Barx5"},
        {"Titanium Leggings"           + "x1" ,       "Gold Barx6"},
        {"Titanium Sword"              + "x1" ,       "Gold Barx5"},
        {"Titanium Chestplate"         + "x1" ,       "Gold Barx7"},
        {"Diamond"                     + "x1" ,       "Gold Barx3"},
        {"Diamond Boots"               + "x1" ,       "Gold Barx6"},
        {"Diamond Leggings"            + "x1" ,       "Gold Barx8"},
        {"Diamond Sword"               + "x1" ,       "Gold Barx7"},
        {"Diamond Chestplate"          + "x1" ,       "Gold Barx10"},
        {"Varium"                      + "x1" ,       "Gold Barx5"},
        {"Varium Boots"                + "x1" ,       "Gold Barx8"},
        {"Varium Leggings"             + "x1" ,       "Gold Barx12"},
        {"Varium Sword"                + "x1" ,       "Gold Barx11"},
        {"Varium Chestplate"           + "x1" ,       "Gold Barx16"},

        // Buy
        {"Gold Barx1",        "Titanium Nugget"                 + "x1"},
        {"Gold Barx4",        "Tungsten Carbide Boots"          + "x1"},
        {"Gold Barx5",        "Titanium Bar"                    + "x1"},
        {"Gold Barx6",        "Tungsten Carbide Leggings"       + "x1"},
        {"Gold Barx8",        "Tungsten Carbide Chestplate"     + "x1"},
        {"Gold Barx9",        "Diamond"                         + "x1"},
        {"Gold Barx10",       "Titanium Boots"                  + "x1"},
        {"Gold Barx11",       "Titanium Sword"                  + "x1"},
        {"Gold Barx12",       "Titanium Leggings"               + "x1"},
        {"Gold Barx14",       "Titanium Chestplate"             + "x1"},
        {"Gold Barx15",       "Diamond Boots"                   + "x1"},
        {"Gold Barx16",       "Varium"                          + "x1"},
        {"Gold Barx18",       "Diamond Sword"                   + "x1"},
        {"Gold Barx20",       "Diamond Leggings"                + "x1"},
        {"Gold Barx24",       "Varium Boots"                    + "x1"},
        {"Gold Barx25",       "Diamond Chestplate"              + "x1"},
        {"Gold Barx33",       "Varium Sword"                    + "x1"},
        {"Gold Barx36",       "Varium Leggings"                 + "x1"},
        {"Gold Barx50",       "Varium Chestplate"               + "x1"}
    };

    Dictionary<string, string> allRancherTradesPossible = new Dictionary<string, string> {
        // Sell  
        {"Zombie Spleen"               + "x4" ,       "Gold Barx2"},
        {"Nyert Meat"                  + "x3" ,       "Gold Barx2"},
        {"Chicken"                     + "x3" ,       "Gold Barx2"},
        {"Goblin Meat"                 + "x3" ,       "Gold Barx2"},
        {"Bread Scrap"                 + "x2" ,       "Gold Barx2"},
        {"Apple"                       + "x2" ,       "Gold Barx2"},

        // Buy
        {"Gold Barx1",        "Zombie Spleen"                   + "x2"},
        {"Gold Barx2",        "Nyert Meat"                      + "x2"},
        {"Gold Barx3",        "Chicken"                         + "x3"},
        {"Gold Barx4",        "Goblin Meat"                     + "x4"},
        {"Gold Barx5",        "Bread Scrap"                     + "x5"},
        {"Gold Barx6",        "Apple"                           + "x6"},
    };

    Dictionary<string, string> allLumberJackTradesPossible = new Dictionary<string, string> {
        // Sell  
        {"Wood Shard"       + "x5" ,       "Gold Barx1"},
        {"Stick"            + "x2" ,       "Gold Barx1"},
        {"Stripped Log"     + "x1" ,       "Gold Barx1"},
        {"Tree Trunk"       + "x1" ,       "Gold Barx3"},

        // Buy
        {"Gold Barx1", "Wood Shard"     + "x2"},
        {"Gold Barx2", "Stick"          + "x2"},
        {"Gold Barx3", "Stripped Log"   + "x2"},
        {"Gold Barx4", "Tree Trunk"     + "x1"},
    };

    Dictionary<string, Sprite> spriteMatcher = new Dictionary<string, Sprite>();

    [SerializeField] List<string> pickItemsFromHere = new List<string>();
    [SerializeField] List<string> currentItemsSelling = new List<string>();

    private void Start() {
        traderType = traderTypes[Random.Range(0, traderTypes.Length)];
        spriteMatcher = new Dictionary<string, Sprite> {
            {"Tungsten Carbide Boots"     , forgerItems[0]},
            {"Tungsten Carbide Leggings"  , forgerItems[1]},
            {"Tungsten Carbide Chestplate", forgerItems[2]},
            {"Titanium Nugget"            , forgerItems[3]},
            {"Titanium Bar"               , forgerItems[4]},
            {"Titanium Boots"             , forgerItems[5]},
            {"Titanium Leggings"          , forgerItems[6]},
            {"Titanium Sword"             , forgerItems[7]},
            {"Titanium Chestplate"        , forgerItems[8]},
            {"Diamond"                    , forgerItems[9]},
            {"Diamond Boots"              , forgerItems[10]},
            {"Diamond Leggings"           , forgerItems[11]},
            {"Diamond Sword"              , forgerItems[12]},
            {"Diamond Chestplate"         , forgerItems[13]},
            {"Varium"                     , forgerItems[14]},
            {"Varium Boots"               , forgerItems[15]},
            {"Varium Leggings"            , forgerItems[16]},
            {"Varium Sword"               , forgerItems[17]},
            {"Varium Chestplate"          , forgerItems[18]},
            {"Gold Bar"                   , forgerItems[19]},
            {"Zombie Spleen"              , forgerItems[20]},
            {"Nyert Meat"                 , forgerItems[21]},
            {"Chicken"                    , forgerItems[22]},
            {"Goblin Meat"                , forgerItems[23]},
            {"Bread Scrap"                , forgerItems[24]},
            {"Apple"                      , forgerItems[25]},
            {"Wood Shard"                 , forgerItems[26]},
            {"Stick"                      , forgerItems[27]},
            {"Stripped Log"               , forgerItems[28]},
            {"Tree Trunk"                 , forgerItems[29]},
        };

        inventory = GameObject.FindObjectOfType<Inventory>();        
        c = GameObject.FindObjectOfType<CraftingSystem>();        

        foreach (GameObject g in tradingWindowObjects) {
            g.transform.localScale = Vector3.zero;
        }

        if (traderType == "Forger") {
            pickItemsFromHere = allForgerTradesPossible.Keys.ToList();
        } else if (traderType == "Rancher") {
            pickItemsFromHere = allRancherTradesPossible.Keys.ToList();
        } else if (traderType == "Lumberjack") {
            pickItemsFromHere = allLumberJackTradesPossible.Keys.ToList();
        }

        for (int i = 0; i < Random.Range(4, 6); i++) {
            int r = Random.Range(0, pickItemsFromHere.Count);
            string pickedItem = pickItemsFromHere[r];
            pickItemsFromHere.Remove(pickedItem);
            currentItemsSelling.Add(pickedItem);
        }
    }

    public void ShowNextTrade() {
        currentIndexShowing++;

        if (currentIndexShowing > currentItemsSelling.Count - 1) {
            currentIndexShowing = 0;
        } 
    }

    public void ShowPreviousTrade() {
        currentIndexShowing--;

        if (currentIndexShowing < 0) {
            currentIndexShowing = currentItemsSelling.Count - 1;
        } 
    }

    public void Trade() {
        if (tradingSlot.Item != null && tradingSlot.Item.ItemName == reactant && tradingSlot.Amount >= reactantAmount) {
            int timesToTrade = tradingSlot.Amount / reactantAmount;
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) {
                for (int i = 0; i < timesToTrade; i++) {
                    TradeItem();
                }       
            } else {
                TradeItem();
            }
        }
    }

    void TradeItem() {
        tradingSlot.Amount -= reactantAmount;

        if (tradingSlot.Amount == 0) {
            tradingSlot.Item = null; 
        }

        Item itemCopy = (Resources.Load("Items/" + product) as Item);  
        for (int j = 0; j < productAmount; j++) {
            inventory.AddItem(itemCopy);
        }
    }

    private void Update() {
        spawnPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        realPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
        RaycastHit2D hit = Physics2D.Raycast(realPoint, Vector2.zero);
        
        #region Showing Trade Interface
        string pureR = currentItemsSelling[currentIndexShowing];

        string pureP; 
        if (traderType == "Forger") {
            pureP = allForgerTradesPossible[currentItemsSelling[currentIndexShowing]];
            p = pureP.Split('x');
        } else if (traderType == "Rancher") {
            pureP = allRancherTradesPossible[currentItemsSelling[currentIndexShowing]];
            p = pureP.Split('x');
        } else if (traderType == "Lumberjack") {
            pureP = allLumberJackTradesPossible[currentItemsSelling[currentIndexShowing]];
            p = pureP.Split('x');
        }
        
        r = pureR.Split('x');   
        reactant = r[0];
        product = p[0];
        reactantAmount = Int32.Parse(r[1]);
        productAmount = Int32.Parse(p[1]);
        reactantAmountText.text = reactantAmount > 1 ? r[1] : "";
        productAmountText.text = productAmount > 1 ? p[1] : "";
        tradeCounterText.text = string.Format("{0}/{1}", currentIndexShowing + 1, currentItemsSelling.Count);
        reactantImage.sprite = spriteMatcher[reactant];
        productImage.sprite = spriteMatcher[product];
        #endregion

        hoveringOver = hit.collider != null ? hit.collider.name : "Nothing";
        hoverOverSelf = hit.collider == GetComponent<Collider2D>() && hit.collider != null;

        if (hoverOverSelf && Input.GetMouseButtonDown(1) && !c.IsTradingMenuActive() && !c.isCraftingMenuActive) {
            OpenTradingMenu();
            c.ToggleCraftingMenu();
            isTradingMenuActive = true;
        } else if (Input.GetKeyDown(KeyCode.C) && c.IsTradingMenuActive()) {
            CloseTradingMenu();
            c.ToggleCraftingMenu();
            isTradingMenuActive = false;
        }

    }

    private void OpenTradingMenu() {
        foreach(GameObject g in tradingWindowObjects) {
            StartCoroutine(LerpMenu(Vector3.zero, Vector3.one * 0.75f, g));
        }
    }

    private void CloseTradingMenu() {
        foreach(GameObject g in tradingWindowObjects) {
            StartCoroutine(LerpMenu(Vector3.one * 0.75f, Vector3.zero, g));
        }
    }

    IEnumerator LerpMenu(Vector3 startScale, Vector3 finalScale, GameObject thing) {
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
