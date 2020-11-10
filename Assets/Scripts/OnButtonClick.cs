using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Handles UI clicks </summary>
public class OnButtonClick : MonoBehaviour {
    public GameObject HUD, map, pauseCanvas, quitCanvas;
    public GameObject winCanvas, endCanvas;
    public GameObject listOfLevels, resetPanel;
    public LevelManager gameManager;
    public Text pauseTime, pauseGems;
    public Text muteButton;

    // Start is called before the first frame update
    void Start() {
        if (SceneManager.GetActiveScene().name == "Welcome") {
            if (Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)))
                muteButton.text = "UNMUTE";
            else
                muteButton.text = "MUTE";
        }
    }

    public void Play() {
        PlayClick();
        //if this is the first launch
        if (PlayerPrefs.GetInt("Run", 0) == 0) {
            LoadScene("Help");
        } else {
            int recentLevel = PlayerPrefs.GetInt("RecentLevel");
            SceneManager.LoadScene("Level " + recentLevel);
        }
    }

    public void NextLevel() {
        PlayClick();
        int currentScene = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
        int nextScene = currentScene + 1;
        if (nextScene > Main.NUM_LEVELS) {
            winCanvas.SetActive(false);
            endCanvas.SetActive(true);
        } else {
            SceneManager.LoadScene("Level " + nextScene);
        }
    }

    public void LoadScene(string name) {
        PlayClick();
        SceneManager.LoadScene(name);
    }

    public void ReloadScene() {
        PlayClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Pause() {
        PlayClick();
        gameManager.gamePaused = true;
        Time.timeScale = 0;
        pauseTime.text = "TIME REMAINING: " + LevelManager.formatTime(gameManager.timeRemaining);
        pauseGems.text = String.Format("GEMS: {0}/{1}", gameManager.gemsCollected, gameManager.gemsToBeCollected);
        pauseCanvas.SetActive(true);
        HUD.SetActive(false);
        StartCoroutine(ShowInterstitialAd(3f));
    }

    IEnumerator ShowInterstitialAd(float delay) {
        yield return new WaitForSecondsRealtime(delay);
        if (Time.timeScale == 0) {  //if the game is still paused after the delay
            //Toast.Show(FindObjectOfType<InterstitialAdManager>().failReason);
            FindObjectOfType<InterstitialAdManager>().ShowAd();
        }
    }

    public void Resume() {
        PlayClick();
        gameManager.gamePaused = false;
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        HUD.SetActive(true);
    }

    public void ShowMap() {
        PlayClick();
        gameManager.gamePaused = true;
        Time.timeScale = 0;
        map.SetActive(true);
        HUD.SetActive(false);
        StartCoroutine(ShowInterstitialAd(3f));
    }

    public void HideMap() {
        PlayClick();
        gameManager.gamePaused = false;
        Time.timeScale = 1;
        map.SetActive(false);
        HUD.SetActive(true);
    }

    public void QuitLevel() {
        PlayClick();
        quitCanvas.SetActive(true);
    }

    public void ConfirmQuitLevel() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Stop("ThemeSong");
        PlayClick();
        LoadScene("Welcome");
    }

    public void CancelQuitLevel() {
        PlayClick();
        quitCanvas.SetActive(false);
    }

    public void QuitGame() {
        PlayClick();
        Application.Quit();
    }

    public void MuteOrUnmute() {
        PlayClick();
        if (Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0))) {
            muteButton.text = "MUTE";
            PlayerPrefs.SetInt("IsMuted", 0);
        }
        else {
            muteButton.text = "UNMUTE";
            PlayerPrefs.SetInt("IsMuted", 1);
        }
    }

    public void ResetProgress() {
        PlayClick();
        listOfLevels.SetActive(false);
        resetPanel.SetActive(true);
    }

    public void ConfirmResetProgress() {
        PlayClick();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CancelResetProgress() {
        PlayClick();
        resetPanel.SetActive(false);
        listOfLevels.SetActive(true);
    }

    void PlayClick() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Click");
    }
}
