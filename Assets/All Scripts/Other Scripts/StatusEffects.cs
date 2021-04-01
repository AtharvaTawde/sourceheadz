using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffects : MonoBehaviour {
    
    [SerializeField] GameObject statusEffectBox;
    [SerializeField] PlayerCombat player;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float[] effects = new float[5];
    [SerializeField] GameObject[] SEFs = new GameObject[5];

    void Update() {
        effects[0] = player.agility;
        effects[1] = player.longevity;
        effects[2] = player.voracity;
        effects[3] = player.hypermobility;
        effects[4] = player.autoimmunity;

        // show the status effect panels with the respective images
        for (int i = 0; i < SEFs.Length; i++) {
            if (effects[i] > 0 && SEFs[i] == null) {

                SEFs[i] = Instantiate(statusEffectBox, transform.position, transform.rotation);
                SEFs[i].transform.parent = transform;
                SEFs[i].transform.localScale = Vector3.one * 0.75f;
                Transform imgObject = SEFs[i].transform.GetChild(0).GetChild(0);
                imgObject.GetComponent<Image>().sprite = sprites[i]; 
            
            } else if (effects[i] <= 0) { 
            
                if (SEFs[i] != null) {
                    Destroy(SEFs[i]);
                }
                SEFs[i] = null;
            
            }
        }

        // show timer
        for (int j = 0; j < SEFs.Length; j++) {
            if (SEFs[j] != null) {
                float seconds = (effects[j] % 60);
                float minutes = (int)((effects[j] / 60) % 60);
                TextMeshProUGUI boxText = SEFs[j].transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
                boxText.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));
            }
        }

    }
}
