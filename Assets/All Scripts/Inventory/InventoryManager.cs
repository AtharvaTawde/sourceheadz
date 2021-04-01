using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    private Inventory inventory;
    private ItemTooltip itemTooltip;
    private Image draggableItem;
    private BaseItemSlot dragItemSlot;

    [SerializeField] int mouseButton;

    public bool ableToDropHoverItem = false;    
    public ItemSlot itemSlotHoverOver;

    private void Awake() {
        inventory = GetComponentInChildren<Inventory>();
        itemTooltip = GetComponentInChildren<ItemTooltip>();
        draggableItem = GameObject.Find("Draggable Item").GetComponent<Image>();
        inventory.OnPointerEnterEvent += ShowTooltip;
        inventory.OnPointerExitEvent += HideTooltip;
        inventory.OnBeginDragEvent += BeginDrag;
        inventory.OnEndDragEvent += EndDrag;
        inventory.OnDragEvent += Drag;
        inventory.OnDropEvent += Drop;

        itemTooltip.HideTooltip();
    }

    private void ShowTooltip(BaseItemSlot itemSlot) {
        itemSlotHoverOver = itemSlot as ItemSlot;
        Item item = itemSlot.Item as Item;
        if (item != null) {
            itemTooltip.ShowTooltip(item);
            ableToDropHoverItem = true;
        }
    }

    private void HideTooltip(BaseItemSlot itemSlot) {
        itemTooltip.HideTooltip();
        ableToDropHoverItem = false;
    }

    private void BeginDrag(BaseItemSlot itemSlot) {
        if (itemSlot.Item != null) {
            dragItemSlot = itemSlot;
            draggableItem.sprite = itemSlot.Item.Icon;
            draggableItem.transform.position = Input.mousePosition;
            draggableItem.enabled = true;
        }
    }

    private void Drag(BaseItemSlot itemSlot) {
        if (dragItemSlot == null) return;
        
        if (draggableItem.enabled) {
            draggableItem.transform.position = Input.mousePosition;
            mouseButton = 2;

            if (Input.GetMouseButton(0)) {
                mouseButton = 0;
            } else if (Input.GetMouseButton(1)) {
                mouseButton = 1;
            }
        }        
    }

    private void EndDrag(BaseItemSlot itemSlot) {    
        dragItemSlot = null;
        draggableItem.enabled = false;
    }

    private void Drop(BaseItemSlot dropItemSlot) {
        if (dragItemSlot == null) return;
 
        if (dropItemSlot.CanAddStack(dragItemSlot.Item) && mouseButton == 0) {
            AddStacks(dropItemSlot);
        } else if (dropItemSlot.CanReceiveItem(dragItemSlot.Item) && dragItemSlot.CanReceiveItem(dropItemSlot.Item) && mouseButton == 0) {
            SwapItems(dropItemSlot);
        } else if (mouseButton == 1) {
            SplitStacks(dropItemSlot);
        }
    }

    private void AddStacks(BaseItemSlot dropItemSlot) {
        if (dragItemSlot.Amount + dropItemSlot.Amount > dropItemSlot.Item.MaximumStacks) {
            int stacksToAdd = dragItemSlot.Item.MaximumStacks - dropItemSlot.Amount;
            dropItemSlot.Amount += stacksToAdd;
            dragItemSlot.Amount -= stacksToAdd;
        } else {
            int stacksToAdd = dragItemSlot.Amount;
            dropItemSlot.Amount += stacksToAdd;
            dragItemSlot.Amount -= stacksToAdd;
        }      
    } 

    private void SwapItems(BaseItemSlot dropItemSlot) {
        Item draggedItem = dragItemSlot.Item;
        int draggedItemAmount = dragItemSlot.Amount;

        dragItemSlot.Item = dropItemSlot.Item;
        dragItemSlot.Amount = dropItemSlot.Amount;

        dropItemSlot.Item = draggedItem;
        dropItemSlot.Amount = draggedItemAmount;
    }

    private void SplitStacks(BaseItemSlot dropItemSlot) {
        if (dropItemSlot.Item == null && dragItemSlot.Amount > 1) {
            int stacksToAdd = Mathf.RoundToInt(dragItemSlot.Amount / 2);
            dragItemSlot.Amount -= stacksToAdd;
            dropItemSlot.Item = dragItemSlot.Item;
            dropItemSlot.Amount += stacksToAdd;
        }
    }

}