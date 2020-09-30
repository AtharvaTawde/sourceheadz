using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour {
    public TextMeshProUGUI textDisplay;
    public Animator transition;
    public Animator texter;
    public float transitionTime = 1f;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    private int scene;
    public GameObject continueButton;

    void Start() {
        scene = SceneManager.GetActiveScene().buildIndex;
        texter.SetTrigger("Animate");
        StartCoroutine(Type());
        if (textDisplay.text == sentences[index]) {
            continueButton.SetActive(true);
        }
    }

    void Update() {
        if (textDisplay.text == sentences[index]) {
            continueButton.SetActive(true);
        }
    }

    IEnumerator Type() {
        foreach(char letter in sentences[index].ToCharArray()) {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence() {
        continueButton.SetActive(false);
        if (index < sentences.Length - 1) {
            index++;
            textDisplay.text = "";
            texter.SetTrigger("Animate");
            StartCoroutine(Type());
        } else {
            textDisplay.text = "";
            continueButton.SetActive(false);
            SkipButton();
        }
    }

    IEnumerator LoadLevel(int levelIndex) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    public void SkipButton() {
        if (scene == 6) {
            StartCoroutine(LoadLevel(1));
        } else if (scene == 7) {
            StartCoroutine(LoadLevel(8));
        } else if (scene == 9) {
            StartCoroutine(LoadLevel(4));
        }
    }
}
