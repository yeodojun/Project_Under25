using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
    public void MainSceneChange() {
        SceneManager.LoadScene("MainScene");
    }

    public void GameMainSceneChange() {
        SceneManager.LoadScene("ShootingScene");
    }
}
