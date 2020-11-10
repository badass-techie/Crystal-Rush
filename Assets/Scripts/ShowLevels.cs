using UnityEngine;
using UnityEngine.UI;

/// <summary> Populates scrollview with levels </summary>
public class ShowLevels : MonoBehaviour {
    public Transform scrollviewContent;

    // Start is called before the first frame update
    void Start() {
        int numLevels = Main.NUM_LEVELS;
        int farthestLevel = PlayerPrefs.GetInt("FarthestLevel");
        for(int i = 0; i < numLevels; ++i) {
            GameObject levelButton = Instantiate(Resources.Load<GameObject>("LevelButton"), scrollviewContent);
            levelButton.GetComponentInChildren<Text>().text = "Level " + (i + 1);
            levelButton.GetComponent<Button>().interactable = i+1 <= farthestLevel;
        }
    }

    // Update is called once per frame 
    void Update() {

    }
}
