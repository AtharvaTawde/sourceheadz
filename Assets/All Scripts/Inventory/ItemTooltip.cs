using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour {
    
    [SerializeField] TextMeshProUGUI ItemNameText;

    public void ShowTooltip(Item item) {
        ItemNameText.text = item.ItemName;
        
        gameObject.SetActive(true);
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }

}
