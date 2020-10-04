using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject restartMenuUI;
    public GameObject gameOverText;

    public static bool gameIsPaused = false;
    public static bool gameEnded = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused && !gameEnded) {
                Resume();
            } else if (!gameEnded) {
                Pause();
            }
        }
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void PauseRestart() {
        restartMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        iTween.ScaleFrom(gameOverText, iTween.Hash(
            "scale", new Vector3(3f,3f,1f),
            "time", 1f,
            "ignoretimescale", true,
            "easetype", iTween.EaseType.easeOutElastic
        ));
        Time.timeScale = 0f;
        gameIsPaused = true;
        gameEnded = true;
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Restart() {
        Time.timeScale = 1f;
        gameIsPaused = false;
        gameEnded = false;
        SceneManager.LoadScene("MainScene");
    }

    public void ToMainMenu() {
        Time.timeScale = 1f;
        gameIsPaused = false;
        gameEnded = false;
        SceneManager.LoadScene("MenuScene");
    }
}
