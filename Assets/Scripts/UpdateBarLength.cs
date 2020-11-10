using System;
using UnityEngine;

/// <summary> Updates the length of bars like health bars </summary>
public class UpdateBarLength : MonoBehaviour {
    public enum t { time, gems }
    public t Type;
    public LevelManager gameManager;

    // Update is called once per frame
    void Update() {
        if (Type == t.gems) {
            transform.localScale = new Vector3(gameManager.gemsCollected / Convert.ToSingle(gameManager.gemsToBeCollected), 1, 1);
        }
        else if (gameManager.timeRemaining >= 0) {
            transform.localScale = new Vector3(gameManager.timeRemaining / Convert.ToSingle(gameManager.timeAllowed), 1, 1);
        }
    }
}
