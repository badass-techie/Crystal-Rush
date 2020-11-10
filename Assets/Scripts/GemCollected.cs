using System;
using UnityEngine;

public class GemCollected : MonoBehaviour {
    LevelManager gameManager;
    
    // Start is called before the first frame update
    void Start() {
        gameManager = GameObject.Find("Game Manager").GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider other) {
        ++gameManager.gemsCollected;
        gameObject.SetActive(false);
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("CollectGem");
        GameObject.Find(gameObject.name + " Icon").SetActive(false);
    }
}
