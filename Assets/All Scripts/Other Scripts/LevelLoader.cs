using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.EventSystems;

public class LevelLoader : MonoBehaviour {

    #region Public Variables
    public Animator transition;
    public Animator playermove;
    public float transitionTime;
    public GameObject MainMenu;
    public GameObject Instrux;
    public Slider qualitySlider;
    public GameObject PostProcessing;   
    //public TextMeshProUGUI storyModeText;
    #endregion
    
    #region Private Variables
    private int scene;
    private string sceneName;
    private bool playerDead;
    private bool thruGate;
    #endregion

    private void Start() {
        PostProcessing = GameObject.Find("Post Processing Stack");
        
        if (qualitySlider != null)
            qualitySlider.value = PlayerPrefs.GetFloat("Quality");
        
        PostProcessing.GetComponent<PostProcessVolume>().weight = PlayerPrefs.GetFloat("Quality");
        scene = SceneManager.GetActiveScene().buildIndex;
        sceneName = SceneManager.GetActiveScene().name;
        ButtonSetup();
    } 

    private void ButtonSetup() {
        GameObject[] things = FindObjectsOfType<GameObject>();

        foreach (GameObject thing in things) {
            if (thing.GetComponent<ButtonSetup>() == null && thing.GetComponent<Button>() != null) {
                thing.AddComponent<ButtonSetup>();
            }

            if (thing.GetComponent<AudioSource>() == null && thing.GetComponent<Button>() != null) {
                thing.AddComponent<AudioSource>();
            }
        }
    }

    private void Update() { 
        if (sceneName == "Selector") {
            PostProcessing.GetComponent<PostProcessVolume>().weight = PlayerPrefs.GetFloat("Quality");
        }
        
        if (qualitySlider != null) {
            PlayerPrefs.SetFloat("Quality", qualitySlider.value);
        }
    }

    #region Toggle Story Mode
    //void ToggleStoryMode() {
    //    if (PlayerPrefs.GetString("Story") == "True" && storyModeText != null) {
    //        storyModeText.text = "Story Mode is ON\nPress M to toggle.";
    //    } else if (PlayerPrefs.GetString("Story") != "True" && storyModeText != null) {
    //        storyModeText.text = "Story Mode is OFF\nPress M to toggle.";
    //    }
    //    if (Input.GetKeyDown(KeyCode.M) && PlayerPrefs.GetString("Story") == "True") {
    //        PlayerPrefs.SetString("Story", "False");
    //    } else if (Input.GetKeyDown(KeyCode.M) && PlayerPrefs.GetString("Story") != "True") {
    //        PlayerPrefs.SetString("Story", "True");
    //    }
    //}
    #endregion
    
    // external calls (such as buttons)
    public void Play() {
        LoadArea("Stage 1");
    }

    public void HTP() {
        LoadInstrux();
    }

    public void Quit() {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void Back() {
        SwitchActive();
    }

    public void LoadArea(string name) {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelByName(name));
    } 

    public void LoadMainMenu() {
        LoadArea("Selector");
    }

    public void LoadWorld() {
        LoadArea("Stage 1");
    }

    public void LoadStatistics() {
        LoadArea("Statistics");
    }

    public void LoadShop() {
        LoadArea("Shop");
    }

    public void LoadSettings() {
        LoadArea("Settings");
    }

    public void LoadInstrux() {
        MainMenu.SetActive(false);
        if (playermove != null) {
            playermove.SetBool("Move", true);
        }  
        Instrux.SetActive(true);
    }

    public void SwitchActive() {
        MainMenu.SetActive(true);   
        if (playermove != null) {
            playermove.SetBool("Move", false);
        }
        Instrux.SetActive(false);
    }

    IEnumerator LoadLevel(int levelIndex) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    IEnumerator LoadLevelByName(string name) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(name);
    }
}