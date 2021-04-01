using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : MonoBehaviour {
    
    [SerializeField] GameObject speechBubble;
    [SerializeField] GameObject itemImage;
    [SerializeField] Sprite[] items;
    [SerializeField] string itemWanted;
    [SerializeField] bool itemIsObtained = false; 

    private string[] itemNames = {"Gold Scrap"                      +"x1", 
                                  "Titanium Bar"                    +"x1", 
                                  "Titanium Nugget"                 +"x1", 
                                  "Titanium Boots"                  +"x1", 
                                  "Titanium Chestplate"             +"x1", 
                                  "Titanium Leggings"               +"x1", 
                                  "Tungsten Carbide Boots"          +"x1", 
                                  "Tungsten Carbide Chestplate"     +"x1", 
                                  "Tungsten Carbide Leggings"       +"x1", 
                                  "Diamond"                         +"x1", 
                                  "Diamond Boots"                   +"x1", 
                                  "Diamond Chestplate"              +"x1", 
                                  "Diamond Leggings"                +"x1", 
                                  "Diamond Sword"                   +"x1", 
                                  "Corpuscle"                       +"x1"};     
    private Vector3 startScale;

    private void Start() {
        int random = Random.Range(0, items.Length - 1); 
        itemImage.GetComponent<SpriteRenderer>().sprite = items[random];
        startScale = speechBubble.transform.localScale;
        speechBubble.transform.localScale = Vector3.zero;
        itemWanted = itemNames[random];
    }

    private void Update() {
        if (speechBubble.transform.localScale == startScale && itemIsObtained) {
            StartCoroutine(LerpSpeechBubble(startScale, Vector3.zero, speechBubble));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {

        Debug.Log(other.gameObject.name);

        if (other.gameObject.tag == "Player" && !itemIsObtained) {
            StartCoroutine(LerpSpeechBubble(Vector3.zero, startScale, speechBubble));
        }

        if (other.gameObject.name == itemWanted && !itemIsObtained) {
            other.gameObject.name = "Can't_Pick_Up_Now";
            StartCoroutine(LerpItem(other.transform.position, transform.position, other.gameObject));
            GameObject item = Instantiate(Resources.Load("Physical Items/Yellow Crystal") as GameObject, transform.position, transform.rotation);
            item.gameObject.name = "Yellow Crystalx1";
            item.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 250f);
            itemIsObtained = true;  
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !itemIsObtained) {
            StartCoroutine(LerpSpeechBubble(startScale, Vector3.zero, speechBubble));
        }
    }

    IEnumerator LerpSpeechBubble(Vector3 startScale, Vector3 finalScale, GameObject thing) {
        float elapsedTime = 0f;
        float waitTime = 0.1f;

        while (elapsedTime < waitTime) {
            thing.transform.localScale = Vector3.Lerp(startScale, finalScale, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.transform.localScale = finalScale;
        yield return null;
    }

    IEnumerator LerpItem(Vector3 startPos, Vector3 finalPos, GameObject thing) {
        float elapsedTime = 0f;
        float waitTime = 2f;
        Vector3 startScale = thing.transform.localScale;

        while (elapsedTime < waitTime) {
            thing.transform.position = Vector3.Lerp(startPos, finalPos, (elapsedTime / waitTime));
            thing.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime; 
            yield return null;
        }

        thing.transform.position = finalPos;
        thing.transform.localScale = Vector3.zero;
        Destroy(thing);
        yield return null;
    }

}
