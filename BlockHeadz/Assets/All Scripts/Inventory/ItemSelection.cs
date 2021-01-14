using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSelection : MonoBehaviour {
    
    [SerializeField] Transform hotBarParentObject;
    [SerializeField] Transform bgHotBarParentObject;
    [SerializeField] List<Image> bgHotBarItemSlots = new List<Image>();
    [SerializeField] List<ItemSlot> hotBarItemSlots = new List<ItemSlot>();
    [SerializeField] ItemSlot chestplateSlot;
    [SerializeField] ItemSlot leggingsSlot;
    [SerializeField] ItemSlot bootsSlot;
    [SerializeField] ItemSlot swordSlot;
    [SerializeField] ItemSlot tradingSlot;
    [SerializeField] Transform selectedItemImage;
    [SerializeField] Inventory inventory;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] CraftingSystem craftingSystem;
    [SerializeField] Transform throwPoint;
    [SerializeField] GameObject itemsParent;
    [SerializeField] ItemSlot[] itemSlots;
 
    private KeyCode[] hotBarKeys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7};
    private string selectedItem;
    private string hoveringOver;
    private bool facingRight;
    private int slotNumber;
    private int currentHealth;
    private int maxHealth;
    private int instanceNumber = 0;
    private PlayerCombat player;
    private Vector3 spawnPoint;
    private Vector3 realPoint;
    private Vector3 blockPlacementPoint;
    private Vector3 seedPlacementPoint;
    private Color virtualColor = new Color(0f, 255f, 250f, 120f/255f);
    private List<ItemSlot> inventorySlots = new List<ItemSlot>();

    public bool isOverWater;
    public bool portalActivated = false;
    public float eatTime;
    public float k_eatTime = 1f;

    #region Images
    [SerializeField] Sprite popperImage;
    [SerializeField] Sprite gunflowerSeedImage; 
    [SerializeField] Sprite treeTrunkImage; 
    [SerializeField] Sprite strippedLogImage; 
    [SerializeField] Sprite stoneBlockImage; 
    [SerializeField] Sprite ovenImage; 
    [SerializeField] Sprite invalidDropLocation;
    [SerializeField] Item waterBottle; 
    [SerializeField] ItemSlot hoverItemSlot;
    [SerializeField] ItemSlot selected;
    #endregion

    #region Physical Items
    [SerializeField] GameObject popper;
    [SerializeField] GameObject gunflowerSeed;
    [SerializeField] GameObject treeTrunk;
    [SerializeField] GameObject strippedLog;
    [SerializeField] GameObject stoneBlock;
    [SerializeField] GameObject oven;
    [SerializeField] GameObject hp;
    [SerializeField] GameObject clayIdolImage;
    [SerializeField] GameObject healthPot;
    [SerializeField] GameObject fireball;
    [SerializeField] GameObject gateway;
    #endregion

    private void Start() {
        slotNumber = 0;
        selectedItem = "";
        player = GetComponent<PlayerCombat>();
        maxHealth = player.maxHealth;
        eatTime = k_eatTime;
        hotBarItemSlots.Clear();
        
        for (int i = 0; i < 7; i++) {
            hotBarItemSlots.Add(hotBarParentObject.GetChild(i).GetComponent<ItemSlot>());
        }

        for (int i = 0; i < 7; i++) {
            bgHotBarItemSlots.Add(bgHotBarParentObject.GetChild(i).GetComponent<Image>());
        }

        if (itemsParent != null) {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        }

        foreach (ItemSlot itemSlot in itemSlots) {
            if (itemSlot.gameObject.name.Contains("Inventory")) {
                inventorySlots.Add(itemSlot);
            }
        }
    }

    private void Update() {
        currentHealth = player.currentHealth;
        spawnPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        realPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
        RaycastHit2D hit = Physics2D.Raycast(realPoint, Vector2.zero);
        facingRight = GetComponent<CharacterController2D>().m_FacingRight;
        blockPlacementPoint = new Vector3(Mathf.Floor(realPoint.x) + 0.5f, Mathf.Floor(realPoint.y) + 0.5f, 0f);
        seedPlacementPoint = new Vector3(Mathf.Floor(realPoint.x) + 0.5f, Mathf.Floor(realPoint.y) + 1f, 0f);
        hoverItemSlot = inventoryManager.itemSlotHoverOver;
        selected = hotBarItemSlots[slotNumber];

        foreach (Trader trader in itemsParent.GetComponentsInChildren<Trader>()) {
            if (trader.isTradingMenuActive) {
                tradingSlot = trader.gameObject.GetComponentInChildren<ItemSlot>();
            }
        }
        
        // Check the exact Gameobject the cursor is on 
        hoveringOver = hit.collider != null ? hit.collider.name : "Nothing";

        #region Check Slots
        // Monitor Keypresses (Slot Number)
        for (int key = 0; key < hotBarKeys.Length; key++) {
            if (Input.GetKeyDown(hotBarKeys[key])) {
                slotNumber = key;
            }
        }

        // Monitor Scroll Wheel
        if (Input.GetAxis("Scroll") > 0f) {
            slotNumber--;

            if (slotNumber < 0) {
                slotNumber = 0;
            }
        } else if (Input.GetAxis("Scroll") < 0f) {
            slotNumber++;

            if (slotNumber > hotBarKeys.Length - 1) {
                slotNumber = hotBarKeys.Length - 1;
            }
        }

        if (hotBarItemSlots[slotNumber].Item != null) {
            selectedItem = hotBarItemSlots[slotNumber].Item.ItemName;
        } else {
            selectedItem = "Nothing";
        }

        for (int slot = 0; slot < bgHotBarItemSlots.Count; slot++) {
            if (slot == slotNumber) {
                bgHotBarItemSlots[slot].color = Color.green;
            } else {
                bgHotBarItemSlots[slot].color = Color.white;
            }
        }
        #endregion

        #region Throw out items
        if (SingleSelectedItemDrop()) {
            
            if (selectedItem != "Health Pot") {
                GameObject item = Instantiate(Resources.Load("Physical Items/" + hotBarItemSlots[slotNumber].Item.ItemName) as GameObject, transform.position, transform.rotation);
                item.gameObject.name = string.Format("{0}x1", selectedItem);
                float throwPower = 250f;
                if (facingRight) {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                } else {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                }
            } else {
                GameObject item = Instantiate(healthPot, transform.position, transform.rotation);
                item.gameObject.name = "Health Potx1";
                float throwPower = 250f;
                if (facingRight) {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                } else {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                }
            }
            
            hotBarItemSlots[slotNumber].Amount--;

            if (hotBarItemSlots[slotNumber].Amount == 0) {
                hotBarItemSlots[slotNumber].Item = null;
            }

        } else if (SingleHoverItemDrop()) {
            
            if (hoverItemSlot.Item.ItemName != "Health Pot") {
                GameObject item = Instantiate(Resources.Load("Physical Items/" + hoverItemSlot.Item.ItemName) as GameObject, transform.position, transform.rotation);
                item.gameObject.name = string.Format("{0}x1", hoverItemSlot.Item.ItemName);
                float throwPower = 250f;
                if (GetComponent<CharacterController2D>().m_FacingRight) {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                } else {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                }
            } else {
                GameObject item = Instantiate(healthPot, transform.position, transform.rotation);
                item.gameObject.name = "Health Potx1";
                float throwPower = 250f;
                if (GetComponent<CharacterController2D>().m_FacingRight) {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                } else {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                }
            }

            hoverItemSlot.Amount--;

            if (hoverItemSlot.Amount == 0) {
                hoverItemSlot.Item = null;
            }
        
        } else if (FullSelectedItemDrop()) {
            
            if (selectedItem != "Health Pot") {
                GameObject item = Instantiate(Resources.Load("Physical Items/" + hotBarItemSlots[slotNumber].Item.ItemName) as GameObject, transform.position, transform.rotation);
                item.gameObject.name = string.Format("{0}x{1}", selectedItem, hotBarItemSlots[slotNumber].Amount);
                float throwPower = 250f;
                if (GetComponent<CharacterController2D>().m_FacingRight) {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                } else {
                    item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                }
            } else {
                for (int i = 0; i < hotBarItemSlots[slotNumber].Amount; i++) {
                    GameObject item = Instantiate(healthPot, transform.position, transform.rotation);
                    item.gameObject.name = "Health Potx1";
                    float throwPower = 250f;
                    if (GetComponent<CharacterController2D>().m_FacingRight) {
                        item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                    } else {
                        item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                    }
                }
            }
            
            hotBarItemSlots[slotNumber].Item = null;
            hotBarItemSlots[slotNumber].Amount = 0;
        
        } else if (FullHoverItemDrop()) {
            
                if (hoverItemSlot.Item.ItemName != "Health Pot") {
                    GameObject item = Instantiate(Resources.Load("Physical Items/" + hoverItemSlot.Item.ItemName) as GameObject, transform.position, transform.rotation);
                    item.gameObject.name = string.Format("{0}x{1}", hoverItemSlot.Item.ItemName, hoverItemSlot.Amount);
                    float throwPower = 250f;
                    if (GetComponent<CharacterController2D>().m_FacingRight) {
                        item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                    } else {
                        item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                    }
                } else {
                    for (int i = 0; i < hoverItemSlot.Amount; i++) {
                        GameObject item = Instantiate(healthPot, transform.position, transform.rotation);
                        item.gameObject.name = "Health Potx1";
                        float throwPower = 250f;
                        if (GetComponent<CharacterController2D>().m_FacingRight) {
                            item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * throwPower);
                        } else {
                            item.GetComponent<Rigidbody2D>().AddForce(Vector2.left * throwPower);
                        }
                    }
                }
            
            hoverItemSlot.Item = null;
            hoverItemSlot.Amount = 0;
        
        }
        #endregion

        #region Move Items
        
        if (SwapItemToEquipmentSlot()) {
            
            if (hoverItemSlot.Item != null && hoverItemSlot.Item.ItemName.Contains("Chestplate")) {
                SwapItems(hoverItemSlot, chestplateSlot);
            } else if (hoverItemSlot.Item != null && hoverItemSlot.Item.ItemName.Contains("Leggings")) {
                SwapItems(hoverItemSlot, leggingsSlot);
            } else if (hoverItemSlot.Item != null && hoverItemSlot.Item.ItemName.Contains("Boots")) {
                SwapItems(hoverItemSlot, bootsSlot);
            } else if (hoverItemSlot.Item != null && hoverItemSlot.Item.ItemName.Contains("Sword")) {
                SwapItems(hoverItemSlot, swordSlot);
            }

        } else if (MoveHotbarItemToInventory()) {
            
            bool added = false;
            foreach (ItemSlot slot in inventorySlots) {
                if (slot.Item != null && slot.Item.ItemName == hoverItemSlot.Item.ItemName) {
                    if (slot.Amount + hoverItemSlot.Amount <= slot.Item.MaximumStacks) {
                        int stacksToAdd = hoverItemSlot.Amount;
                        slot.Amount += stacksToAdd;
                        hoverItemSlot.Amount -= stacksToAdd;
                        added = true;
                        break;
                    } else if (slot.Amount + hoverItemSlot.Amount > slot.Item.MaximumStacks && slot.Amount != slot.Item.MaximumStacks) {
                        int stacksToAdd = hoverItemSlot.Item.MaximumStacks - slot.Amount;
                        slot.Amount += stacksToAdd;
                        hoverItemSlot.Amount -= stacksToAdd;
                        break;
                    }
                }
            }

            if (!added) {
                foreach (ItemSlot slot in inventorySlots) {
                    if (slot.Item == null) {
                        slot.Item = hoverItemSlot.Item;
                        slot.Amount = hoverItemSlot.Amount;
                        hoverItemSlot.Item = null;
                        hoverItemSlot.Amount = 0;
                        break;
                    }
                }
            }
        } else if (MoveInventoryItemToHotbar()) {
            
            bool added = false;
            foreach (ItemSlot slot in hotBarItemSlots) {
                if (slot.Item != null && slot.Item.ItemName == hoverItemSlot.Item.ItemName) {
                    if (slot.Amount + hoverItemSlot.Amount <= slot.Item.MaximumStacks) {
                        int stacksToAdd = hoverItemSlot.Amount;
                        slot.Amount += stacksToAdd;
                        hoverItemSlot.Amount -= stacksToAdd;
                        added = true;
                        break;
                    } else if (slot.Amount + hoverItemSlot.Amount > slot.Item.MaximumStacks && slot.Amount != slot.Item.MaximumStacks) {
                        int stacksToAdd = hoverItemSlot.Item.MaximumStacks - slot.Amount;
                        slot.Amount += stacksToAdd;
                        hoverItemSlot.Amount -= stacksToAdd;
                        break;
                    }
                }
            }

            if (!added) {
                foreach (ItemSlot slot in hotBarItemSlots) {
                    if (slot.Item == null) {
                        slot.Item = hoverItemSlot.Item;
                        slot.Amount = hoverItemSlot.Amount;
                        hoverItemSlot.Item = null;
                        hoverItemSlot.Amount = 0;
                        break;
                    }
                }
            }
        
        } else if (TakeOffArmor()) {
        
            bool added = false;
            foreach (ItemSlot slot in hotBarItemSlots) {
                if (slot.Item == null) {
                    slot.Item = hoverItemSlot.Item;
                    slot.Amount = hoverItemSlot.Amount;
                    hoverItemSlot.Item = null;
                    hoverItemSlot.Amount = 0;
                    added = true;
                    break;
                }
            }

            if (!added) {
                foreach (ItemSlot slot in inventorySlots) {
                    if (slot.Item == null) {
                        slot.Item = hoverItemSlot.Item;
                        slot.Amount = hoverItemSlot.Amount;
                        hoverItemSlot.Item = null;
                        hoverItemSlot.Amount = 0;
                        added = true;
                        break;
                    }
                }
            }
        
        } else if (RemoveItemFromCrafting() || RemoveFromTradingMenu()) {
        
            bool added = false;
            foreach (ItemSlot slot in hotBarItemSlots) {
                if (slot.Item != null && slot.Item.ItemName == hoverItemSlot.Item.ItemName) {
                    if (slot.Amount + hoverItemSlot.Amount <= slot.Item.MaximumStacks) {
                        int stacksToAdd = hoverItemSlot.Amount;
                        slot.Amount += stacksToAdd;
                        hoverItemSlot.Amount -= stacksToAdd;
                        added = true;
                        break;
                    } else if (slot.Amount + hoverItemSlot.Amount > slot.Item.MaximumStacks && slot.Amount != slot.Item.MaximumStacks) {
                        int stacksToAdd = hoverItemSlot.Item.MaximumStacks - slot.Amount;
                        slot.Amount += stacksToAdd;
                        hoverItemSlot.Amount -= stacksToAdd;
                        break;
                    }
                }
            }

            if (!added) {
                foreach (ItemSlot slot in inventorySlots) {
                    if (slot.Item != null && slot.Item.ItemName == hoverItemSlot.Item.ItemName) {
                        if (slot.Amount + hoverItemSlot.Amount <= slot.Item.MaximumStacks) {
                            int stacksToAdd = hoverItemSlot.Amount;
                            slot.Amount += stacksToAdd;
                            hoverItemSlot.Amount -= stacksToAdd;
                            added = true;
                            break;
                        } else if (slot.Amount + hoverItemSlot.Amount > slot.Item.MaximumStacks && slot.Amount != slot.Item.MaximumStacks) {
                            int stacksToAdd = hoverItemSlot.Item.MaximumStacks - slot.Amount;
                            slot.Amount += stacksToAdd;
                            hoverItemSlot.Amount -= stacksToAdd;
                            break;
                        }
                    }
                }
            }

            if (!added) {
                foreach (ItemSlot slot in hotBarItemSlots) {
                    if (slot.Item == null) {
                        slot.Item = hoverItemSlot.Item;
                        slot.Amount = hoverItemSlot.Amount;
                        hoverItemSlot.Item = null;
                        hoverItemSlot.Amount = 0;
                        added = true;
                    }
                }
            }


            if (!added) {
                foreach (ItemSlot slot in inventorySlots) {
                    if (slot.Item == null) {
                        slot.Item = hoverItemSlot.Item;
                        slot.Amount = hoverItemSlot.Amount;
                        hoverItemSlot.Item = null;
                        hoverItemSlot.Amount = 0;
                        added = true;
                    }
                }
            }
        
        } else if (AddToTradingMenu()) {

            if (tradingSlot.Item != null && tradingSlot.Item.ItemName == hoverItemSlot.Item.ItemName) {
                if (tradingSlot.Amount + hoverItemSlot.Amount <= tradingSlot.Item.MaximumStacks) {
                    int stacksToAdd = hoverItemSlot.Amount;
                    tradingSlot.Amount += stacksToAdd;
                    hoverItemSlot.Amount -= stacksToAdd;
                } else if (tradingSlot.Amount + hoverItemSlot.Amount > tradingSlot.Item.MaximumStacks && tradingSlot.Amount != tradingSlot.Item.MaximumStacks) {
                    int stacksToAdd = hoverItemSlot.Item.MaximumStacks - tradingSlot.Amount;
                    tradingSlot.Amount += stacksToAdd;
                    hoverItemSlot.Amount -= stacksToAdd;
                }
            } else {
                SwapItems(hoverItemSlot, tradingSlot);
            }

        }

        #endregion

        #region Usable Items (Single Press)
        if (Input.GetMouseButtonDown(1) && !HelperFunctions.IsPointerOverUIElement() && !craftingSystem.isCraftingMenuActive) {
            if (selectedItem == "Popper" && !hoveringOver.Contains("Chunk") && !hoveringOver.Contains("Oven") && !hoveringOver.Contains("Trading Post")) {
            
                selected.Amount--;
                GameObject popperInstance = Instantiate(popper, realPoint, transform.rotation);
            
            } else if (selectedItem == "Clay Idol" && !hoveringOver.Contains("Chunk") && !hoveringOver.Contains("Oven") && !hoveringOver.Contains("Stratum") && !hoveringOver.Contains("Trading Post")) {
            
                selected.Amount--;
                StartCoroutine(ClayIdolThrow(realPoint, 0.2f));
            
            } else if (selectedItem == "Boss Gem" && hoveringOver.Contains("Gateway")) {
            
                selected.Amount--;
                portalActivated = true;
                StartCoroutine(LerpScale(Vector3.zero, Vector3.one * 2, gateway));

            } else if (selectedItem == "Fireball") {
            
                selected.Amount--;
                FireballThrow();
            
            } else if (selectedItem == "Empty Bottle" && hoveringOver.Contains("Pool")) {

                selected.Item = waterBottle;
                selected.Amount = 1;

            } else if (selectedItem.Contains("Chestplate") && !craftingSystem.isCraftingMenuActive && !hoveringOver.Contains("Oven")) {
                
                SwapItems(selected, chestplateSlot);
                
            } else if (selectedItem.Contains("Leggings") && !craftingSystem.isCraftingMenuActive && !hoveringOver.Contains("Oven")) {
                
                SwapItems(selected, leggingsSlot);

            } else if (selectedItem.Contains("Boots") && !craftingSystem.isCraftingMenuActive && !hoveringOver.Contains("Oven")) {
                
                SwapItems(selected, bootsSlot);

            } else if (selectedItem.Contains("Sword") && !craftingSystem.isCraftingMenuActive && !hoveringOver.Contains("Oven")) {
                
                SwapItems(selected, swordSlot);

            }

            if (selected.Item == null) {
                selected.Amount = 0;
            }
        }
        #endregion

        #region Usable Items (Hold Press)
        
        if (Input.GetMouseButton(1) && !HelperFunctions.IsPointerOverUIElement() && !craftingSystem.isCraftingMenuActive) {            
            if (selectedItem == "Gunflower Seed" && hoveringOver == "Nothing") {
                
                if (selectedItemImage.GetComponent<SpriteRenderer>().sprite == gunflowerSeedImage) {
                    hotBarItemSlots[slotNumber].Amount--;
                    GameObject gunflowerSeedInstance = Instantiate(gunflowerSeed, selectedItemImage.position, transform.rotation);
                    HelperFunctions.IncrementInt("Gunflowers Planted");
                }
            
            } else if (selectedItem == "Tree Trunk" && hoveringOver == "Nothing") {
            
                hotBarItemSlots[slotNumber].Amount--;
                GameObject instance = Instantiate(treeTrunk, blockPlacementPoint, transform.rotation) as GameObject;
                instance.name += instanceNumber;
                instanceNumber += 1;
                GetComponent<ChunkLoad>().blocks.Add(instance);

            } else if (selectedItem == "Stripped Log" && hoveringOver == "Nothing") {
            
                hotBarItemSlots[slotNumber].Amount--;
                GameObject instance = Instantiate(strippedLog, blockPlacementPoint, transform.rotation) as GameObject;
                instance.name += instanceNumber;
                instanceNumber += 1;
                GetComponent<ChunkLoad>().blocks.Add(instance);
                
            } else if (selectedItem == "Stone Block" && hoveringOver == "Nothing") {
            
                hotBarItemSlots[slotNumber].Amount--;
                GameObject instance = Instantiate(stoneBlock, blockPlacementPoint, transform.rotation) as GameObject;
                instance.name += instanceNumber;
                instanceNumber += 1;
                GetComponent<ChunkLoad>().blocks.Add(instance);
                
            } else if (selectedItem == "Oven" && hoveringOver == "Nothing") {
            
                hotBarItemSlots[slotNumber].Amount--;
                GameObject instance = Instantiate(oven, blockPlacementPoint, transform.rotation) as GameObject;
                instance.name += instanceNumber;
                instanceNumber += 1;
                GetComponent<ChunkLoad>().blocks.Add(instance);
                
            } 
        }
        #endregion

        #region Eat Food (Hold)
        if (Input.GetMouseButton(1) && !HelperFunctions.IsPointerOverUIElement() && isFood() && !craftingSystem.isCraftingMenuActive) {
            if (eatTime > 0 && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                eatTime -= Time.deltaTime;
            } else if (eatTime <= 0) {
                eatTime = 0;
                if (selectedItem == "Health Pot") {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHealth += 50;
                    eatTime = k_eatTime;

                } else if (selectedItem == "Blood Pie" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 60.00f;             // c = 1.5s             (currentHunger)
                    player.saturation += 40.00f;                // s = 1.25x            (saturation)
                    StartCoroutine(Heal(32));                   // h = x                (healValue)
                    eatTime = k_eatTime;

                } else if (selectedItem == "Cooked Goblin Meat" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {

                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 30.00f;
                    player.saturation = 20.00f;
                    StartCoroutine(Heal(16));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Cooked Chicken" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {

                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 28.125f;
                    player.saturation = 18.75f;
                    StartCoroutine(Heal(15));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Cooked Nyert Meat" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {

                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 26.25f;
                    player.saturation = 17.50f;
                    StartCoroutine(Heal(14));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Cooked Zombie Spleen" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 22.50f;
                    player.saturation += 15.00f;
                    StartCoroutine(Heal(12));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Apple" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 18.75f;
                    player.saturation += 12.50f;
                    StartCoroutine(Heal(10));
                    eatTime = k_eatTime;
                
                } else if (selectedItem == "Bread Scrap" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {

                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 15.00f;
                    player.saturation = 10.00f;
                    StartCoroutine(Heal(8));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Goblin Meat" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 7.500f;
                    player.saturation += 5.000f;
                    StartCoroutine(Heal(4));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Chicken" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 5.625f;
                    player.saturation += 3.750f;
                    StartCoroutine(Heal(3));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Nyert Meat" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 3.750f;
                    player.saturation += 2.500f;
                    StartCoroutine(Heal(2));
                    eatTime = k_eatTime;

                } else if (selectedItem == "Zombie Spleen" && player.currentHunger != player.maxHunger && !hoveringOver.Contains("Oven")) {
                
                    StartCoroutine(player.HealthPotDrinkParticles());
                    hotBarItemSlots[slotNumber].Amount--;
                    player.currentHunger += 1.875f;
                    player.saturation += 1.250f;
                    StartCoroutine(Heal(1));
                    eatTime = k_eatTime;

                }
            }
        } else if (isFood() && !Input.GetMouseButton(1)) {
            eatTime = k_eatTime;
        }
        #endregion

        PotentialDrop();
    }

    void PotentialDrop() {
        if (selectedItem == "Popper") {
        
            if (hoveringOver == "Nothing") {
                selectedItemImage.position = realPoint;
                selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
                selectedItemImage.localScale = new Vector3(1, 1, 1);
                selectedItemImage.GetComponent<SpriteRenderer>().sprite = popperImage;
            } else {
                selectedItemImage.position = realPoint;
                ShowInvalidDropSpot();
            }
        
        } else if (selectedItem == "Gunflower Seed") {
            
            RaycastHit2D plantSpot = Physics2D.Raycast(selectedItemImage.position, Vector2.down);
            if (hoveringOver == "Nothing") {
                if (plantSpot.collider.tag == "Ground") {
                    selectedItemImage.GetComponent<SpriteRenderer>().sprite = gunflowerSeedImage;
                    selectedItemImage.position = new Vector3(seedPlacementPoint.x, plantSpot.point.y + 1, 0f);
                    selectedItemImage.localScale = new Vector3(1, 1, 1);
                    selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
                } else {
                    selectedItemImage.position = realPoint;
                    ShowInvalidDropSpot();
                }
            } else {
                selectedItemImage.position = realPoint;
                ShowInvalidDropSpot();
            }  
        
        } else if (selectedItem == "Tree Trunk") { 
         
            if (hoveringOver == "Nothing") {
                selectedItemImage.GetComponent<SpriteRenderer>().sprite = treeTrunkImage;
                selectedItemImage.position = blockPlacementPoint;
                selectedItemImage.localScale = new Vector3(1, 1, 1);
                selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
            } else {
                selectedItemImage.position = realPoint;
                ShowInvalidDropSpot();
            }
        
        } else if (selectedItem == "Stripped Log") { 
         
            if (hoveringOver == "Nothing") {
                selectedItemImage.GetComponent<SpriteRenderer>().sprite = strippedLogImage;
                selectedItemImage.position = blockPlacementPoint;
                selectedItemImage.localScale = new Vector3(1, 1, 1);
                selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
            } else {
                selectedItemImage.position = realPoint;
                ShowInvalidDropSpot();
            }
        
        } else if (selectedItem == "Stone Block") { 
         
            if (hoveringOver == "Nothing") {
                selectedItemImage.GetComponent<SpriteRenderer>().sprite = stoneBlockImage;
                selectedItemImage.position = blockPlacementPoint;
                selectedItemImage.localScale = new Vector3(1, 1, 1);
                selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
            } else {
                selectedItemImage.position = realPoint;
                ShowInvalidDropSpot();
            }

        } else if (selectedItem == "Oven") { 
         
            if (hoveringOver == "Nothing") {
                selectedItemImage.GetComponent<SpriteRenderer>().sprite = ovenImage;
                selectedItemImage.position = blockPlacementPoint;
                selectedItemImage.localScale = new Vector3(1, 1, 1);
                selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
            } else {
                selectedItemImage.position = realPoint;
                ShowInvalidDropSpot();
            }
        
        } else {
            selectedItemImage.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }
 
    void ShowInvalidDropSpot() {
        selectedItemImage.GetComponent<SpriteRenderer>().sprite = invalidDropLocation;
        selectedItemImage.localScale = new Vector3(4, 4, 4);
        selectedItemImage.GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator ClayIdolThrow(Vector3 targetPosition, float duration) {
        GameObject thrownClayIdol = Instantiate(clayIdolImage, transform.position, transform.rotation) as GameObject;
        float time = 0;
        Vector3 startPosition = thrownClayIdol.transform.position;
        while (time < duration) {
            thrownClayIdol.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            thrownClayIdol.transform.eulerAngles = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 0, 360), time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        thrownClayIdol.transform.position = targetPosition;
        transform.position = thrownClayIdol.transform.position;
        Destroy(thrownClayIdol);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    void FireballThrow() {
        GameObject thrownFireball = Instantiate(fireball, throwPoint.position, transform.rotation) as GameObject;
        thrownFireball.name = "Player_Fireball";
        Vector3 throwDirection = throwPoint.position - realPoint;
        thrownFireball.GetComponent<Rigidbody2D>().AddForce(-throwDirection * 250f);
    }

    public IEnumerator HpParticles() {
        GameObject plusInstance = Instantiate(hp, new Vector3(transform.position.x, transform.position.y + 0.87f - 2f, transform.position.z), transform.rotation);
        yield return new WaitForSeconds(2f);
        Destroy(plusInstance);
    }

    bool SingleSelectedItemDrop() {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Q) && hotBarItemSlots[slotNumber].Item != null && !craftingSystem.isCraftingMenuActive) {
            return true;
        } else {
            return false;
        }
    }

    bool SingleHoverItemDrop() {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Q) && inventoryManager.ableToDropHoverItem && craftingSystem.isCraftingMenuActive) {
            return true;
        } else {
            return false;
        }
    }

    bool FullSelectedItemDrop() {
        if (Shifting() && Input.GetKeyDown(KeyCode.Q) && hotBarItemSlots[slotNumber].Item != null && !craftingSystem.isCraftingMenuActive) {
            return true;
        } else {
            return false;
        }
    }

    bool FullHoverItemDrop() {
        if (Shifting() && Input.GetKeyDown(KeyCode.Q) && inventoryManager.ableToDropHoverItem && craftingSystem.isCraftingMenuActive) {
            return true;
        } else {
            return false;
        }
    }

    bool slotIsPartOf(string n) {
        if (hoverItemSlot.gameObject != null) {
            return hoverItemSlot.gameObject.name.Contains(n);
        } else {
            return false; 
        }
    }

    bool Shifting() {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    bool MoveHotbarItemToInventory() {
        return Shifting() 
        && Input.GetMouseButtonDown(0) 
        && slotIsPartOf("Hotbar") 
        && inventoryManager.ableToDropHoverItem 
        && craftingSystem.isCraftingMenuActive
        && !craftingSystem.IsTradingMenuActive();
    }

    bool MoveInventoryItemToHotbar() {
        return Shifting() 
        && Input.GetMouseButtonDown(0) 
        && slotIsPartOf("Inventory") 
        && inventoryManager.ableToDropHoverItem 
        && craftingSystem.isCraftingMenuActive
        && !craftingSystem.IsTradingMenuActive();
    }

    bool SwapItemToEquipmentSlot() {
        return Shifting() 
        && Input.GetMouseButtonDown(0) 
        && (slotIsPartOf("Inventory") || slotIsPartOf("Hotbar")) 
        && inventoryManager.ableToDropHoverItem 
        && craftingSystem.isCraftingMenuActive 
        && (hoverItemSlot.Item.ItemName.Contains("Chestplate") || hoverItemSlot.Item.ItemName.Contains("Leggings") || hoverItemSlot.Item.ItemName.Contains("Boots") || hoverItemSlot.Item.ItemName.Contains("Sword"))
        && !craftingSystem.IsTradingMenuActive();
    }

    bool TakeOffArmor() {
        return Shifting() 
        && Input.GetMouseButtonDown(0) 
        && slotIsPartOf("Equipment")
        && inventoryManager.ableToDropHoverItem 
        && craftingSystem.isCraftingMenuActive;
    }
    
    bool RemoveItemFromCrafting() {
        return Shifting() 
        && Input.GetMouseButtonDown(0) 
        && slotIsPartOf("Crafting") 
        && inventoryManager.ableToDropHoverItem 
        && craftingSystem.isCraftingMenuActive;
    }

    bool AddToTradingMenu() {
        return Shifting() 
        && Input.GetMouseButtonDown(0)
        && (slotIsPartOf("Hotbar") || slotIsPartOf("Inventory")) 
        && inventoryManager.ableToDropHoverItem
        && craftingSystem.IsTradingMenuActive();
    }

    bool RemoveFromTradingMenu() {
        return Shifting() 
        && Input.GetMouseButtonDown(0)
        && slotIsPartOf("Trading")
        && inventoryManager.ableToDropHoverItem
        && craftingSystem.IsTradingMenuActive();
    }

    public IEnumerator Heal(int healAmount) {
        float interval = 0.1f;
        for (int i = 0; i < healAmount; i++) {
            player.currentHealth += 5;
            if (player.currentHealth >= player.maxHealth) {
                player.currentHealth = player.maxHealth;
            }
            yield return new WaitForSeconds(interval);
            interval *= 1.01f;
        }
    }

    void SwapItems(ItemSlot start, ItemSlot end) {
        Item item = end.Item;
        int amount = end.Amount;
        end.Item = start.Item;
        end.Amount = start.Amount;
        
        if (item != null) {
            start.Item = item;
            start.Amount = amount;
        } else {
            start.Item = null;
            start.Amount = 0;
        }
    }

    public bool isFood() {
        string[] foods = {"Health Pot", "Blood Pie", "Goblin Meat", 
                          "Nyert Meat", "Apple", "Bread Scrap", 
                          "Cooked Goblin Meat", "Cooked Nyert Meat", 
                          "Zombie Spleen", "Cooked Zombie Spleen", 
                          "Chicken", "Cooked Chicken"};

        foreach (string food in foods) {
            if (food == selectedItem) {
                return true;
            }
        }
        return false;
    }

    IEnumerator LerpScale(Vector3 startScale, Vector3 finalScale, GameObject thing) {
        float elapsedTime = 0f;
        float waitTime = 0.1f;

        while (elapsedTime < waitTime) {
            thing.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.transform.localScale = finalScale;
        yield return null;
    }

}
