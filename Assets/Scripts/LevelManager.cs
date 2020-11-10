using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Level Manager - duh! </summary>
public class LevelManager : MonoBehaviour {
    [HideInInspector] public int gemsCollected;
    public int gemsToBeCollected;
    public int timeAllowed;
    public float fallThreshold;
    [HideInInspector] public int timeRemaining;

    public GameObject winCanvas, loseCanvas, HUD, aura;
    public Transform player;
    public Text scoreLabel, timeLabel, winLabel, highScoreLabel;
    public AnchorPlayerToIsland anchor;
    [HideInInspector] public bool gamePaused = false;
    Transform mainCamera;

    string gameStatus = "running";   //running, won, or lost
    string deathBy = "";  //how the player died
    
    public static Func<int, string> formatTime = time => string.Format("{0}:{1}", Convert.ToInt32(Math.Floor(time / 60d)), (time % 60).ToString("00"));
    
    // Start is called before the first frame update
    void Start() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("ThemeSong");
        mainCamera = Camera.main.transform;
        timeRemaining = timeAllowed;
        StartCoroutine(Countdown());
        StartCoroutine(Flash());

        //update stored variables
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
        PlayerPrefs.SetInt("RecentLevel", currentLevel);
    }

    // Update is called once per frame
    void Update() {
        scoreLabel.text = String.Format("GEMS: {0}/{1}", gemsCollected, gemsToBeCollected);
        timeLabel.text = "TIME: " + formatTime(timeRemaining);

        //the game has been won
        if (gemsCollected >= gemsToBeCollected) {
            if (gameStatus == "running") {
                gameStatus = "won";
                gamePaused = true;

                foreach (GameObject island in GameObject.FindGameObjectsWithTag("Island")) {
                    if (island.GetComponent<MoveIsland>() != null)
                        island.GetComponent<MoveIsland>().moveIsland = false;
                }
                aura.transform.position = player.position;
                aura.transform.parent = player;
                aura.SetActive(true);

                int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
                int farthestLevel = PlayerPrefs.GetInt("FarthestLevel");
                if (currentLevel + 1 <= Main.NUM_LEVELS && farthestLevel < currentLevel + 1)
                    PlayerPrefs.SetInt("FarthestLevel", currentLevel + 1);

                int time = timeAllowed - timeRemaining;
                int previousHighScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + " HighScore", 0);
                int newHighScore = time < previousHighScore || previousHighScore == 0 ? time : previousHighScore;
                if (newHighScore < previousHighScore && previousHighScore != 0)
                    Toast.Show("New high score :)", Toast.Position.top, 3);
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + " HighScore", newHighScore);
                winLabel.text = "TIME ELAPSED: " + formatTime(time);
                highScoreLabel.text = "BEST: " + formatTime(newHighScore);

                HUD.SetActive(false);
                winCanvas.SetActive(true);
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false) {
                    FindObjectOfType<AudioManager>().Stop("ThemeSong");
                    FindObjectOfType<AudioManager>().Play("Win");
                }
            }
        }
        
        //the game has been lost
        if (timeRemaining <= 0 || anchor.distanceInAir < fallThreshold) {
            if (gameStatus == "running") {
                gameStatus = "lost";
                gamePaused = true;
                HUD.SetActive(false);
                loseCanvas.SetActive(true);
                if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false) {
                    FindObjectOfType<AudioManager>().Stop("ThemeSong");
                }

                if (timeRemaining >= 0) {
                    deathBy = "running out of time";
                    player.gameObject.SetActive(false);
                }
                if (anchor.distanceInAir < fallThreshold)
                    deathBy = "falling";
            }
        }
    }

    IEnumerator Countdown() {
        while (true) {
            yield return new WaitForSeconds(1);
            if (gameStatus == "running")
                --timeRemaining;
        }
    }

    IEnumerator Flash() {
        while (true) {
            if (timeRemaining <= 10 && timeRemaining > 0) {
                timeLabel.enabled = !timeLabel.enabled;
                timeLabel.color = Color.red;
            }
            else {
                timeLabel.enabled = true;
                timeLabel.color = scoreLabel.color;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Respawn() {
        player.gameObject.SetActive(true);
        HUD.SetActive(true);
        loseCanvas.SetActive(false);
        FindObjectOfType<Joystick>().ResetJoystickPosition();
        if (deathBy == "falling") {
            //print(anchor.lastIslandInContact.gameObject.name);
            player.parent.parent = anchor.lastIslandInContact;
            player.position = anchor.lastIslandInContact.position + Vector3.up * 0.1f;  //place player slightly higher so that they don't go through the collider
            mainCamera.position = player.position;
            anchor.distanceInAir = 0;
        } else if (deathBy == "running out of time") {
            timeRemaining += 30;
            if (player.parent.parent == null) {
                //if player is mid air 
                player.parent.parent = anchor.lastIslandInContact;
                player.position = anchor.lastIslandInContact.position + Vector3.up * 0.1f;  //place player slightly higher so that they don't go through the collider
                mainCamera.position = player.position;
                anchor.distanceInAir = 0;
            }
        }
        gameStatus = "running";
        gamePaused = false;
        deathBy = "";
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false) {
            FindObjectOfType<AudioManager>().Play("ThemeSong");
        }
    }

    public void DisplayAd() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Click");
        if (FindObjectOfType<RewardedAdManager>().ad.IsLoaded()) {
            FindObjectOfType<RewardedAdManager>().ShowAd();
        }
        else {
            if (FindObjectOfType<RewardedAdManager>().failedToLoad) {
                //Toast.Show(FindObjectOfType<RewardedAdManager>().errorMessage, Toast.Position.bottom, 3f);
                string err = FindObjectOfType<RewardedAdManager>().errorMessage;
                if (err == "Internal error")
                    Toast.Show("Check your internet connection", Toast.Position.bottom, 3f);
                else if (err == "No fill")
                    Toast.Show("No ads for now", Toast.Position.bottom, 3f);
                else
                    Toast.Show("Something went wrong", Toast.Position.bottom, 3f);
                FindObjectOfType<RewardedAdManager>().ReloadAd();
            }
            else {
                Toast.Show("Try again in a few seconds...", Toast.Position.bottom, 3f);
            }
        }
    }
}
