using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {

    public void LoadMainGame() {
        SceneManager.LoadScene(2);
    }

    public void LoadLeaderBoard() {
        SceneManager.LoadScene(1);
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
    }
}
