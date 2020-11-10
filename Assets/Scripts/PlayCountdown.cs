using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayCountdown : MonoBehaviour {
    public Text label;
    public float countdown;


    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        label.text = string.Format("PLAY ({0})", Mathf.Ceil(countdown));
        if(countdown <= 0) {
            Play();
        }
        countdown -= Time.deltaTime;
    }

    public void LoadPlay() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Click");
        Play();
    }

    void Play() {
        int recentLevel = PlayerPrefs.GetInt("RecentLevel");
        SceneManager.LoadScene("Level " + recentLevel);
    }

    public void LoadMenu() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene("Welcome");
    }
}
