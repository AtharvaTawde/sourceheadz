using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BaseItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    [SerializeField] protected Image image;
    [SerializeField] protected TextMeshProUGUI amountText; 

    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;
    public event Action<BaseItemSlot> OnRightClickEvent;

    protected Color normalColor = Color.white;
    protected Color disabledColor = Color.clear; 

    protected Item _item;
    public Item Item {
        get { return _item; }
        set {
            _item = value;

            if (_item == null && Amount != 0) Amount = 0;

            if (_item == null) {
                image.sprite = null;
                image.color = disabledColor;
            } else {
                image.sprite = _item.Icon;
                image.color = normalColor;
            }
        }
    }

    private int _amount;
    public int Amount {
        get {return _amount; }
        set {
            _amount = value;
            if (_amount < 0) _amount = 0;
            if (_amount == 0 && Item != null) Item = null;

            if (amountText != null) {
                amountText.enabled = _item != null && _amount > 1;
                if (amountText.enabled) {
                    amountText.text = _amount.ToString();
                }
            }                
        }
    }

    public virtual bool CanAddStack(Item item, int amount = 1) {
        return Item != null && Item.ID == item.ID && Amount + amount <= item.MaximumStacks;
    }

    protected virtual void OnValidate() {
        if (image == null) {
            image = GetComponent<Image>();
        }

        if (amountText == null) {
            amountText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }
    
    public virtual bool CanReceiveItem(Item item) {
        return false;
    }

    private void Start() {
        if (image == null) {
            image = GetComponent<Image>();
        }

        if (amountText == null) {
            amountText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        OnPointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnPointerExitEvent?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnRightClickEvent?.Invoke(this);
    }

}
