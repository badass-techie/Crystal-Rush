using UnityEngine;

public class UIButtonController : MonoBehaviour {
    bool buttonDown = false;

    public void SetButtonDown(bool trueOrFalse) {
        buttonDown = trueOrFalse;
    }

    public bool GetButtonDown() {
        return buttonDown;
    }
}
