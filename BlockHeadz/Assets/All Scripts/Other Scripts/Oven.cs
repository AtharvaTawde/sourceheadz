using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Oven : MonoBehaviour {
    
    [SerializeField] float fuel = 0; 
    [SerializeField] float fuelConsumed;
    [SerializeField] TextMeshPro fuelCounter;    
    [SerializeField] bool selected = false;   
    private Dictionary<string, string> foodTable = new Dictionary<string, string> {
        {"Nyert Meat",      "Cooked Nyert Meat"}, 
        {"Goblin Meat",     "Cooked Goblin Meat"}, 
        {"Zombie Spleen",   "Cooked Zombie Spleen"},
        {"Chicken",         "Cooked Chicken"},
    };

    private Dictionary<string, float> fuelTable = new Dictionary<string, float> {
        {"Tree Trunk",      6f}, 
        {"Coal Dust",       4f}, 
        {"Stripped Log",    3f}, 
        {"Stick",           2f}, 
        {"Wood Shard",      1f}
    };
    
    public List<string> foodInOven = new List<string>();
    public List<string> fuelInOven = new List<string>();
    [SerializeField] float timer;
    private const float k_heatTime = 5f;

    private void Start() {
        timer = k_heatTime;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Fuel
        foreach (KeyValuePair<string, float> entry in fuelTable) {
            if (other.gameObject.name.Contains(entry.Key) && selected) {
                string[] info = other.gameObject.name.Split('x');
                for (int i = 0; i < Int32.Parse(info[1]); i++) {
                    fuelInOven.Add(info[0]);
                    fuel += entry.Value;
                }

                Destroy(other.gameObject);
            }
        }

        // Food
        foreach (string rawFood in foodTable.Keys.ToArray()) {
            string[] info = other.gameObject.name.Split('x');
            if (info[0] == rawFood && selected) {
                for (int i = 0; i < Int32.Parse(info[1]); i++) {
                    foodInOven.Add(rawFood);
                }
                                
                Destroy(other.gameObject);
            }
        }

    }

    private void Update() {
        Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 realPoint = new Vector3(spawnPoint.x, spawnPoint.y, 0f);
        RaycastHit2D hit = Physics2D.Raycast(realPoint, Vector2.zero);

        // Check the exact Gameobject the cursor is on 
        bool hoverOverSelf;
        string hoveringOver;

        hoveringOver = hit.collider != null ? hit.collider.name : "Nothing";
        hoverOverSelf = hit.collider == GetComponent<Collider2D>() ? true : false;

        if (fuel < 0) {
            fuel = 0;
        }
        
        // Choose Oven
        if (!selected && hoverOverSelf && !IsPointerOverUIElement() && Input.GetMouseButtonDown(1)) {
            GetComponent<SpriteRenderer>().color = Color.white;
            selected = true;
        } else if (selected && hoveringOver.Contains("Oven") && !IsPointerOverUIElement() && Input.GetMouseButtonDown(1)) {
            GetComponent<SpriteRenderer>().color = Color.grey;
            selected = false;
        }

        // Oven Logic
        fuelCounter.text = fuel.ToString();
        if (fuel >= 1 && foodInOven.Count >= 1) {   
            string cookedItem = foodTable[foodInOven[0]];
            string currentFuelItem;
            
            currentFuelItem = fuelInOven.Count > 0 ? currentFuelItem = fuelInOven[0] : "Nothing";

            if (timer > 0) {
                timer -= Time.deltaTime;
            } else if (timer <= 0) {
                timer = 0;
                fuel--;
                fuelConsumed--;
                foodInOven.RemoveAt(0);
                GameObject item = Instantiate(Resources.Load("Physical Items/" + cookedItem) as GameObject, transform.position, transform.rotation);
                item.gameObject.name = string.Format("{0}x1", cookedItem);
                timer = k_heatTime;
            }

            // Consume and add fuel values
            if (fuelInOven.Count > 0) {
                foreach (KeyValuePair<string, float> entry in fuelTable) {
                    if (currentFuelItem.Contains(entry.Key) && fuelConsumed == 0) {
                        fuelInOven.RemoveAt(0);
                        fuelConsumed += entry.Value;
                    }
                }
            }

        }
    }
    #region Is Pointer Over UI Element?
    public static bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for(int index = 0; index < eventSystemRaysastResults.Count; index ++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults [index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults() {   
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    #endregion

}
