using System.Collections.Generic;
using UnityEngine;

/// <summary> 
///     Moves the joystick this script is attached to, and tracks its position on devices with touch input.
///     The joystick's handle must be the child of this joystick
/// </summary>
public class Joystick : MonoBehaviour {
    Camera mainCamera;
    
    RectTransform joystick;
    RectTransform joystickHandle;

    float handleMovementRange;
    [Header("Joystick")]
    [Tooltip("Controls the extent to which the handle will move within the joystick")]
    public float handleCoeff;

    Vector2 movementZone;
    [Header("Movement Region")]
    [Tooltip("Bounds of the region in which we'll track the finger that moves the joystick")]
    public Vector2 bounds;

    Vector3 initialJoystickPosition;
    List<TouchInfo> touches;    //all touches in area of movement in the order in which they happened

    public LevelManager gameManager;

    class TouchInfo {
        public Touch touch;
        public Vector3 touchStart;
        public Vector3 lastTouchInsideMovementZone = Vector3.zero;

        public TouchInfo() {
        }
        public TouchInfo(Touch t, Vector3 s) {
            touch = t;
            touchStart = s;
        }
    }

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
        joystick = transform.gameObject.GetComponent<RectTransform>();
        joystickHandle = joystick.GetChild(0).gameObject.GetComponent<RectTransform>();
        Vector3[] joystickRectTransformCorners = new Vector3[4];
        joystick.GetWorldCorners(joystickRectTransformCorners);
        handleMovementRange = handleCoeff * (joystickRectTransformCorners[3].x - joystickRectTransformCorners[0].x) / 2;
        movementZone = new Vector2(mainCamera.pixelWidth * bounds.x, mainCamera.pixelHeight * bounds.y);
        initialJoystickPosition = joystick.position;
        touches = new List<TouchInfo>();
    }

    void Update() {
        if (!gameManager.gamePaused) {
            // Iterate through all the detected touches
            for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++) {
                Touch currentTouch = Input.GetTouch(touchIndex);
                bool touchInsideMovementZone = currentTouch.position.x < movementZone.x && currentTouch.position.y < movementZone.y;

                // Check each touch's phase
                switch (currentTouch.phase) {
                    case TouchPhase.Began:
                        if (touchInsideMovementZone) {
                            touches.Add(new TouchInfo(currentTouch, currentTouch.position));
                            StartJoystickMovement(touches[touches.Count - 1]);
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled: {
                            int index = touches.FindIndex(t => t.touch.fingerId == currentTouch.fingerId);    //returns default object if object to search is not found
                            if (index > -1) {   //if object was found in list
                                TouchInfo touchToRemove = touches[index];
                                touches.Remove(touchToRemove);
                                if (touches.Count > 0)
                                    PositionJoystick(touches[touches.Count - 1]);
                                else
                                    ResetJoystickPosition();
                            }
                        }
                        break;
                    case TouchPhase.Moved: {
                            int index = touches.FindIndex(t => t.touch.fingerId == currentTouch.fingerId);    //returns default object if object to search is not found
                            TouchInfo touch = touches[index];
                            if (touchInsideMovementZone && index > -1)
                                touch.lastTouchInsideMovementZone = currentTouch.position;
                            if (touches.IndexOf(touch) == touches.Count - 1)    //if this is the most recent touch
                                MoveHandle(touch, currentTouch);
                        }
                        break;
                    case TouchPhase.Stationary: {
                            int index = touches.FindIndex(t => t.touch.fingerId == currentTouch.fingerId);    //returns default object if object to search is not found
                            TouchInfo touch = touches[index];
                            if (touchInsideMovementZone && index > -1)
                                touch.lastTouchInsideMovementZone = currentTouch.position;
                        }
                        break;
                }
            }
        }
    }

    void StartJoystickMovement(TouchInfo t) {
        joystick.position = t.touch.position;
        joystickHandle.localPosition = Vector3.zero;
    }

    void PositionJoystick(TouchInfo t) {
        joystick.position = t.lastTouchInsideMovementZone;
        joystickHandle.localPosition = Vector3.zero;
    }

    void MoveHandle(TouchInfo currentTouchInfo, Touch currentTouch) {
        Vector3 currentTouchPosition = new Vector3(currentTouch.position.x, currentTouch.position.y);
        Vector3 deltaSinceTouchStart = currentTouchPosition - currentTouchInfo.touchStart;
        if (deltaSinceTouchStart.magnitude < handleMovementRange)
            joystickHandle.position += new Vector3(currentTouch.deltaPosition.x, currentTouch.deltaPosition.y);
        else
            joystickHandle.localPosition = (currentTouchPosition - joystick.position).normalized * handleMovementRange;
    }

    public void ResetJoystickPosition() {
        joystick.position = initialJoystickPosition;
        joystickHandle.localPosition = Vector3.zero;
    }

    public float GetAxis(string axis) {
        if (axis == "Horizontal" && !gameManager.gamePaused)
            return joystickHandle.localPosition.x / handleMovementRange;
        else if (axis == "Vertical" && !gameManager.gamePaused)
            return joystickHandle.localPosition.y / handleMovementRange;
        else
            return 0f;
    }
}
