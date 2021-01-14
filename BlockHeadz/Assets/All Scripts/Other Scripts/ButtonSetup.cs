using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonSetup : MonoBehaviour {
    
    public AudioSource audioSource;
    public TextMeshProUGUI unlockPanelText;
    public int hostileMobCount;

    private AudioClip hoverSound, clickSound;    
    private GameObject unlockPanel;
    private string sceneName;
    private GameObject[] items = new GameObject[8]; // actual number of items + 1

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        sceneName = SceneManager.GetActiveScene().name;
        hoverSound = Resources.Load("hover") as AudioClip;
        clickSound = Resources.Load("click") as AudioClip;

        if (sceneName == "Shop") {
            unlockPanel = GameObject.Find("Shop/Unlock Panel");
            for (int i = 0; i < items.Length; i++) {
                items[i] = GameObject.Find("Shop/Item" + i);
            }
            AssignShopListeners();
            if (unlockPanel != null) {
                unlockPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
            }    
        } else if (sceneName != "Shop") {
            AssignRegularListeners();
        }
    }

    public void HoverSound(BaseEventData eventData) {
        audioSource.PlayOneShot(hoverSound);
    }

    public void ClickSound(BaseEventData eventData) {
        audioSource.PlayOneShot(clickSound);
    }

    public void EnterLockedItem(BaseEventData eventData) {
        
        hostileMobCount = PlayerPrefs.GetInt("Goblins Killed") + 
                          PlayerPrefs.GetInt("Zombies Killed") + 
                          PlayerPrefs.GetInt("Archers Killed") + 
                          PlayerPrefs.GetInt("Revenants Killed") +
                          PlayerPrefs.GetInt("Firetraps Killed") + 
                          PlayerPrefs.GetInt("Fallens Killed");
        
        string[] unlockStatements = {"", 
                                     "Kill " + (100 - hostileMobCount).ToString() + " More Hostile Mobs", 
                                     "Kill " + (500 - hostileMobCount).ToString() + " More Hostile Mobs", 
                                     "Kill " + (1000 - hostileMobCount).ToString() + " More Hostile Mobs", 
                                     "Kill " + (5000 - hostileMobCount).ToString() + " More Hostile Mobs", 
                                     "Kill " + (10000 - hostileMobCount).ToString() + " More Hostile Mobs", 
                                     "Activate the Boss Portal",
                                     "Visit Bartie"};

        for (int i = 0; i < items.Length; i++) {
            if (gameObject.name == "Item" + i) { 
                if (items[i].GetComponent<Button>().interactable) {
                    unlockPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
                    unlockPanelText.text = "";
                } else {
                    unlockPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 100f/255f);
                    unlockPanelText.text = unlockStatements[i];
                }
            }
        }
    }

    public void ExitLockedItem(BaseEventData eventData) {
        unlockPanel.GetComponent<Image>().color = new Color(255f, 255f, 255f, 0f);
        unlockPanelText.text = "";
    }

    void AssignShopListeners() {
        for (int i = 0; i < items.Length; i++) {
            AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerEnter, HoverSound);
            AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerEnter, EnterLockedItem);
            AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerClick, ClickSound);
            AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerExit, ExitLockedItem);
        }
    }

    void AssignRegularListeners() {
        if (gameObject.GetComponent<EventTrigger>() == null) {
            gameObject.AddComponent<EventTrigger>();
        }        
        AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerEnter, HoverSound);
        AddEventTriggerListener(GetComponent<EventTrigger>(), EventTriggerType.PointerClick, ClickSound);
    }

    public static void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback) {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }
}
