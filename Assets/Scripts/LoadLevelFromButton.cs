using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevelFromButton : MonoBehaviour {
    public void Load(Text level) {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene(level.text);
    }
}
