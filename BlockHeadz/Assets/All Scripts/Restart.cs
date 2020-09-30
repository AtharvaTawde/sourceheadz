using UnityEngine;
using UnityEngine.SceneManagement;
 
public class Restart : MonoBehaviour {
    
    GameObject player;

    private void Start() {
        player = GameObject.Find("Player");
    }
    
    void Update () {
        if(Input.GetKeyDown(KeyCode.Q) && player != null && !player.GetComponent<PlayerMovement>().isPaused) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}