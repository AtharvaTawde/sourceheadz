using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class InanimateObject : MonoBehaviour {

    [SerializeField] Transform top;
    [SerializeField] Transform bottom;
    
    private Dictionary<string, int> rockDrops = new Dictionary<string, int> {
        {"Stone", 100},
        {"Stone1", 75},
        {"Stone2", 50}
    };

    private Dictionary<string, int> eggDrops = new Dictionary<string, int> {
        {"Egg", 100}
    };

    private Dictionary<string, int> titaniumDrops = new Dictionary<string, int> {
        {"Titanium Nugget", 25}, 
        {"Titanium Nugget1", 50}, 
        {"Titanium Nugget2", 100}
    };

    private bool isTGA;
    private bool isTGB;
    private bool isCheckComplete;

    private void Start() {
        if (!isCheckComplete) {
            if (IsOverlapping()) {
                Destroy(gameObject);
            }
            isCheckComplete = true;    
        }
    }

    public void DestroySelf() {
        if (gameObject.name.Contains("Rock")) {
            GenerateDrops(rockDrops);
            Destroy(gameObject);
        } else if (gameObject.name.Contains("Egg")) {
            GenerateDrops(eggDrops);
            Destroy(gameObject);
        } else if (gameObject.name.Contains("Titanium")) {
            GenerateDrops(titaniumDrops);
            Destroy(gameObject);
        }
    }

    void GenerateDrops(Dictionary<string, int> dict) {
        foreach (KeyValuePair<string, int> entry in dict) {
            int random = Mathf.RoundToInt(Random.Range(0, 100));

            if (random <= entry.Value) {
                Drop(entry.Key.RemoveIntegers());
            }
        }
    }

    void Drop(string name) {
        GameObject drop = Resources.Load("Physical Items/" + name) as GameObject;
        GameObject dropInstance = Instantiate(drop, transform.position, transform.rotation);   
        dropInstance.name = string.Format("{0}x{1}", name, 1);     
    }

    public bool IsOverlapping() {
        Collider2D[] above = Physics2D.OverlapCircleAll(top.position, 0.10f);
        Collider2D[] below = Physics2D.OverlapCircleAll(bottom.position, 0.10f);

        for (int a = 0; a < above.Length; a++) {
            if (above[a].gameObject.tag == "Ground") {
                isTGA = true;
                break;
            }
        }

        for (int b = 0; b < below.Length; b++) {
            if (below[b].gameObject.tag == "Ground") {
                isTGB = true;
                break;
            }
        }

        if (isTGA || !isTGB) {
            return true;
        } else {
            return false;
        }
    }

}
