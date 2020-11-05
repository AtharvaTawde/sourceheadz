using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : BaseItemSlot, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {

    public event Action<BaseItemSlot> OnBeginDragEvent;
    public event Action<BaseItemSlot> OnEndDragEvent;
    public event Action<BaseItemSlot> OnDragEvent;
    public event Action<BaseItemSlot> OnDropEvent;

    private Color dragColor = new Color(1f, 1f, 1f, 0.5f);
    
    public override bool CanReceiveItem(Item item) {
        return true;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        
        if (Item != null)
            image.color = dragColor;
        
        OnBeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData) {
        OnDragEvent?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData) {

        if (Item != null) 
            image.color = normalColor;

        OnEndDragEvent?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData) {
        OnDropEvent?.Invoke(this);
    }

}
