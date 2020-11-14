using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSelection : MonoBehaviour {
    
    [SerializeField] Transform hotBarParentObject;
    [SerializeField] Transform bgHotBarParentObject;
    [SerializeField] List<ItemSlot> hotBarItemSlots = new List<ItemSlot>();
    [SerializeField] List<Image> bgHotBarItemSlots = new List<Image>();
    [SerializeField] Transform selectedItemImage;

    private KeyCode[] hotBarKeys = {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7};
    private string selectedItem;
    private string hoveringOver;
    private int slotNumber;
    private PlayerCombat player;
    private Vector3 spawnPoint;
    private Vector3 realPoint;
    private Color virtualColor = new Color(0f, 255f, 250f, 120f/255f);

    #region Images
    [SerializeField] Sprite popperImage;
    [SerializeField] Sprite gunflowerSeedStage1Image; 
    [SerializeField] Sprite invalidDropLocation;
    #endregion

    #region Physical Items
    [SerializeField] GameObject popper;
    [SerializeField] GameObject gunflowerSeedStage1;
    #endregion

    private void Start() {
        slotNumber = 0;
        selectedItem = "";
        player = GetComponent<PlayerCombat>();
        hotBarItemSlots.Clear();
        int childrenCount = transform.childCount;

        for (int i = 0; i < childrenCount; i++) {
            hotBarItemSlots.Add(hotBarParentObject.GetChild(i).GetComponent<ItemSlot>());
        }

        for (int i = 0; i < childrenCount; i++) {
            bgHotBarItemSlots.Add(bgHotBarParentObject.GetChild(i).GetComponent<Image>());
        }
    }

    private void Update() {
        // Check the exact Gameobject the cursor is on 
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
 
        if (hit.collider != null) {
            hoveringOver = hit.collider.name;
        } else if (hit.collider == null) {
            hoveringOver = "Nothing";
        }

        // Monitor Keypresses (Slot Number)
        for (int key = 0; key < hotBarKeys.Length; key++) {
            if (Input.GetKeyDown(hotBarKeys[key])) {
                slotNumber = key;
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

        spawnPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        realPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0f);

        // Usable Items
        if (Input.GetMouseButtonDown(1) && !IsPointerOverUIElement()) {
            if (selectedItem == "Health Pot") {
                hotBarItemSlots[slotNumber].Amount--;
                StartCoroutine(GetComponent<PlayerCombat>().HealthPotDrinkParticles());
                StartCoroutine(GetComponent<PlayerCombat>().Drinky());
            } else if (selectedItem == "Popper" && hoveringOver == "Nothing") {
                hotBarItemSlots[slotNumber].Amount--;
                GameObject popperInstance = Instantiate(popper, realPoint, transform.rotation);
            } else if (selectedItem == "Clay Idol") {
                hotBarItemSlots[slotNumber].Amount--;
                transform.position = new Vector3(-7, 0, 0);
            } else if (selectedItem == "Gunflower Seed" && hoveringOver == "Nothing") {
                hotBarItemSlots[slotNumber].Amount--;
                GunflowerSeedLogistics();
            }
        }
        
        // Show Potential Drop
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
            if (hoveringOver == "Nothing") {
                selectedItemImage.position = realPoint;
                RaycastHit2D plantSpot = Physics2D.Raycast(selectedItemImage.position, Vector2.down);
            
                if (plantSpot.collider.tag == "Ground") {
                    if (Vector3.Distance(selectedItemImage.position, plantSpot.point) <= 5f) {
                        selectedItemImage.GetComponent<SpriteRenderer>().sprite = gunflowerSeedStage1Image;
                        selectedItemImage.position = new Vector3(plantSpot.point.x, plantSpot.point.y + 1, 0f);
                        selectedItemImage.localScale = new Vector3(1, 1, 1);
                        selectedItemImage.GetComponent<SpriteRenderer>().color = virtualColor;
                    } else {
                        ShowInvalidDropSpot();
                    }
                } else {
                    ShowInvalidDropSpot();
                }
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

    void GunflowerSeedLogistics() {
        GameObject gunflowerSeedStage1Instance = Instantiate(gunflowerSeedStage1, realPoint, transform.rotation);
        RaycastHit2D plantSpot = Physics2D.Raycast(gunflowerSeedStage1Instance.transform.position, Vector2.down);
        if (plantSpot.collider.tag == "Ground") {
            if (Vector3.Distance(gunflowerSeedStage1Instance.transform.position, plantSpot.point) <= 5f) {
                gunflowerSeedStage1Instance.transform.position = new Vector3(plantSpot.point.x, plantSpot.point.y + 1, 0f);
            } else {
                hotBarItemSlots[slotNumber].Amount++;
                Destroy(gunflowerSeedStage1Instance);
            }
        } else {
            hotBarItemSlots[slotNumber].Amount++;
            Destroy(gunflowerSeedStage1Instance);
        }
    }

    #region Is Pointer Over UI Element?
    public static bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for(int index = 0;  index < eventSystemRaysastResults.Count; index ++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults() {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position =  Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    #endregion

}
