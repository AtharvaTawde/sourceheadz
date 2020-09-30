using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class LevelLoader : MonoBehaviour {


    #region Public Variables
    public Animator transition;
    public Animator playermove;
    public float transitionTime;
    public GameObject MainMenu;
    public GameObject Instrux;
    public Slider qualitySlider;
    public GameObject PostProcessing;
    public TextMeshProUGUI storyModeText;
    #endregion
    
    #region Private Variables
    private int scene;
    private string sceneName;
    private bool playerDead;
    private bool thruGate;
    
    GameObject PauseMenu;
    #endregion

    void Start() {
        PostProcessing = GameObject.Find("Post Processing Stack");
        
        if (qualitySlider != null)
            qualitySlider.value = PlayerPrefs.GetFloat("Quality");
        
        if (PlayerPrefs.GetString("Story") == "")
            PlayerPrefs.SetString("Story", "True");

        PostProcessing.GetComponent<PostProcessVolume>().weight = PlayerPrefs.GetFloat("Quality");
        scene = SceneManager.GetActiveScene().buildIndex;
        sceneName = SceneManager.GetActiveScene().name;
    } 

    public void Play() {
        if (PlayerPrefs.GetString("Story") == "True") {
            LoadArea("Story 1");
        } else {
            LoadArea("Stage 1");
        }
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
        StartCoroutine(LoadLevelByName(name));
    } 

    void Update() { 
        PostProcessing.GetComponent<PostProcessVolume>().weight = PlayerPrefs.GetFloat("Quality");
        
        if (qualitySlider != null) {
            PlayerPrefs.SetFloat("Quality", qualitySlider.value);
        }

        if (PlayerPrefs.GetString("Story") == "True" && storyModeText != null) {
            storyModeText.text = "Story Mode is ON\nPress M to toggle.";
        } else if (PlayerPrefs.GetString("Story") != "True" && storyModeText != null) {
            storyModeText.text = "Story Mode is OFF\nPress M to toggle.";
        }

        if (Input.GetKeyDown(KeyCode.M) && PlayerPrefs.GetString("Story") == "True") {
            PlayerPrefs.SetString("Story", "False");
        } else if (Input.GetKeyDown(KeyCode.M) && PlayerPrefs.GetString("Story") != "True") {
            PlayerPrefs.SetString("Story", "True");
        }

        if (scene == 1 || scene == 3 || scene == 5 || scene == 8) {
            if (GameObject.Find("Player") == null) {
                playerDead = true;
            } else {
                playerDead = false;
            }

            if (playerDead) {
                GameOver();
            }

            GameObject player = GameObject.Find("Player");
            PlayerCombat p = player.GetComponent<PlayerCombat>();
            thruGate = p.thruGate;

            if (thruGate) {
                if (sceneName == "Stage 1") {
                    LoadArea("Stage 2");
                } else if (sceneName == "Stage 2") {
                    LoadArea("Stage 3");
                } else if (sceneName == "Stage 3") {
                    if (PlayerPrefs.GetString("Story") == "True") {
                        LoadArea("Story 2");
                    } else {
                        LoadArea("FinalStage");
                    }
                } else if (sceneName == "FinalStage") {
                    if (PlayerPrefs.GetString("Story") == "True") {
                        LoadArea("Story 3");
                    } else {
                        LoadArea("GameComplete");
                    }
                }
            }
        } else if (sceneName == "Game Over" || sceneName == "GameComplete") {
            if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)) {
                LoadMainMenu();
            }
        }
    }
    
    public void LoadMainMenu() {
        Time.timeScale = 1f;
        LoadArea("Selector");
    }

    public void LoadStage1() {
        LoadArea("Stage 1");
    }

    public void GameOver() {
        LoadArea("Game Over");
    }

    public void LoadStage2() {
        LoadArea("Stage 2");
    }

    public void LoadStage3() {
        LoadArea("Stage 3");
    }

    public void LoadStory1() {
        LoadArea("Story 1");
    }

    public void LoadStory2() {
        LoadArea("Story 2");
    }

    public void LoadFinalStage() {
        LoadArea("FinalStage");
    }

    public void LoadFinalStory() {
        LoadArea("Story 3");
    }

    public void LoadStatistics() {
        LoadArea("Statistics");
    }

    public void LoadInstrux() {
        MainMenu.SetActive(false);
        if (playermove != null)    
            playermove.SetBool("Move", true);
        Instrux.SetActive(true);
    }

    public void LoadSettings() {
        LoadArea("Settings");
    }

    public void SwitchActive() {
        MainMenu.SetActive(true);
        if (playermove != null)
            playermove.SetBool("Move", false);
        Instrux.SetActive(false);
    }

    public void GameComplete() {
        LoadArea("GameComplete");
    }

    public void LoadShop() {
        LoadArea("Shop");
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



//SceneManager.GetActiveScene().buildIndex + 1