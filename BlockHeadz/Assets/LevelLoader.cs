using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {

    public Animator transition;
    public float transitionTime = 1f;
    private int scene;
    private bool playerDead;
    private bool thruGate;
    public Button PlayButton;
    public Button InstructionsButton;
    public Button QuitButton;
    public Button BackButton;
    public GameObject MainMenu;
    public GameObject Instrux;
    
    void Start() {
        scene = SceneManager.GetActiveScene().buildIndex;
        Button play = PlayButton.GetComponent<Button>();
        play.onClick.AddListener(Play);
        Button htp = InstructionsButton.GetComponent<Button>();
        htp.onClick.AddListener(HTP);
        Button quit = QuitButton.GetComponent<Button>();
        quit.onClick.AddListener(Quit);
        Button back = BackButton.GetComponent<Button>();
        back.onClick.AddListener(Back); 
    } 

    void Play() {
        LoadGame();
    }

    void HTP() {
        LoadInstrux();
    }

    void Quit() {
        Application.Quit();
    }

    void Back() {
        SwitchActive();
    }

    void Update() { 
        if (scene == 1 || scene == 3) {
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
                if (scene == 1) {
                    LoadStage2();
                } else if (scene == 3) {
                    GameComplete();
                }
            }

        } else if (scene == 2) {
            if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)) {
                LoadMainMenu();
            }
        } else if (scene == 4) {
            if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Space)) {
                LoadMainMenu();
            }
        } 
    }
    
    void LoadMainMenu() {
        StartCoroutine(LoadLevel(0));
    }

    void LoadGame() {
        StartCoroutine(LoadLevel(1));
    }

    void GameOver() {
        StartCoroutine(LoadLevel(2));
    }

    void LoadStage2() {
        StartCoroutine(LoadLevel(3));
    }

    void LoadInstrux() {
        MainMenu.SetActive(false);
        Instrux.SetActive(true);
    }

    void SwitchActive() {
        MainMenu.SetActive(true);
        Instrux.SetActive(false);
    }

    void GameComplete() {
        StartCoroutine(LoadLevel(4));
    }
    
    IEnumerator LoadLevel(int levelIndex) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}


//SceneManager.GetActiveScene().buildIndex + 1