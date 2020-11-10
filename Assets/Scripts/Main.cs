using UnityEngine;

/// <summary> Runs at launch </summary>
public class Main : MonoBehaviour {
    static public int NUM_LEVELS = 7;

    // Start is called before the first frame update
    void Start() {
        if (PlayerPrefs.GetInt("FarthestLevel", 0) == 0)
            PlayerPrefs.SetInt("FarthestLevel", 1);
        if (PlayerPrefs.GetInt("RecentLevel", 0) == 0)
            PlayerPrefs.SetInt("RecentLevel", 1);
    }

    // Update is called once per frame
    void Update() {

    }
}
