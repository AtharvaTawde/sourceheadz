using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour, IItemContainer {
    
    [SerializeField] Item[] startingItems;
    [SerializeField] GameObject inventoryParent;
    [SerializeField] List<ItemSlot> itemSlots;
    [SerializeField] List<ItemSlot> tempChestContainer;  

    public event Action<BaseItemSlot> OnRightClickEvent;
    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;
    public event Action<BaseItemSlot> OnBeginDragEvent;
    public event Action<BaseItemSlot> OnEndDragEvent;
    public event Action<BaseItemSlot> OnDragEvent;       
    public event Action<BaseItemSlot> OnDropEvent;

    private void Start() {
        SetItemSlots();
        SetStartingItems();
        
        for (int i = 0; i < itemSlots.Count; i++) {
            itemSlots[i].OnRightClickEvent += OnRightClickEvent;
            itemSlots[i].OnPointerEnterEvent += OnPointerEnterEvent;
            itemSlots[i].OnPointerExitEvent += OnPointerExitEvent;
            itemSlots[i].OnBeginDragEvent += OnBeginDragEvent;
            itemSlots[i].OnEndDragEvent += OnEndDragEvent;
            itemSlots[i].OnDragEvent += OnDragEvent;
            itemSlots[i].OnDropEvent += OnDropEvent;
        }
    }

    private void OnValidate() {
        SetItemSlots();
        SetStartingItems();
    }

    private void SetItemSlots() {
        itemSlots.Clear();
        tempChestContainer.Clear();
        foreach (ItemSlot itemSlot in inventoryParent.GetComponentsInChildren<ItemSlot>()) {
            if (!itemSlot.name.Contains("Chest")) {
                itemSlots.Add(itemSlot);
            } else {
                tempChestContainer.Add(itemSlot);
            }
        }

        foreach (ItemSlot chestSlot in tempChestContainer) {
            itemSlots.Add(chestSlot);
        }
    }

    private void SetStartingItems() {
        Clear();
        foreach (Item item in startingItems) {
            AddItem(item);
        }
    }

    public bool ContainsItem(Item item) {
        for (int i = 0; i < itemSlots.Count; i++) {
            if (itemSlots[i].Item == item) {
                return true;
            }
        }
        return false;
    } 

    public bool AddItem(Item item) {
        int maxInventorySlots = 31;
        for (int i = 0; i < maxInventorySlots; i++) {
            if (itemSlots[i].CanAddStack(item)) {
                itemSlots[i].Item = item;
                itemSlots[i].Amount++;
                return true;
            }
        }
        
        for (int i = 0; i < maxInventorySlots; i++) {
            if (itemSlots[i].Item == null) {
                itemSlots[i].Item = item;
                itemSlots[i].Amount++;
                return true;
            }
        }
        return false;
    }

    public bool RemoveItem(Item item) {
        for (int i = 0; i < itemSlots.Count; i++) {
            if (itemSlots[i].Item == item) {
                
                itemSlots[i].Amount--;
                if (itemSlots[i].Amount == 0) {
                    itemSlots[i].Item = null;
                }

                return true;
            }
        }
        return false;
    }

    public void Clear() {
        for (int i = 0; i < itemSlots.Count; i++) {
            if (itemSlots[i].Item != null) {
                itemSlots[i].Item = null;
            }
        }
    }

    public Item RemoveItem(string itemID) {
        for (int i = 0; i < itemSlots.Count; i++) {
            Item item = itemSlots[i].Item;
            if (item != null && item.ID == itemID) {
                
                itemSlots[i].Amount--;
                if (itemSlots[i].Amount == 0) {
                    itemSlots[i].Item = null;
                }
                
                return item;
            }
        }
        return null;
    }

    public bool IsFull() {
        for (int i = 0; i < itemSlots.Count; i++) {
            if (itemSlots[i].Item == null) {
                return false;
            }
        }
        return true;
    }

    public int ItemCount(string itemID) {
        int number = 0;
        for (int i = 0; i < itemSlots.Count; i++) {
            if (itemSlots[i].Item.ID == itemID) {
                return number++;
            }
        }
        return number;
    }

}
