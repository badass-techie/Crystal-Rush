using System.Collections.Generic;
using UnityEngine;

/// <summary> Enables panning on devices with touch input </summary>
public class PanScene : MonoBehaviour {
    [Tooltip("The main canvas")]
    public Transform canvas;
    Vector3 touchStart, touchDelta;
    Camera mainCamera;
    Vector2 deadZone;
    [Tooltip("Bounds of the region which we'll ignore")]
    public Vector2 bounds;
    List<Vector3[]> cornersOfUIElements;
    public Joystick joystick;
    public LevelManager gameManager;

    // Start is called before the first frame update
    void Start() {
        mainCamera = Camera.main;
        deadZone = new Vector2(mainCamera.pixelWidth * bounds.x, mainCamera.pixelHeight * bounds.y);
        
        //gets bounds of every ui element on the canvas
        cornersOfUIElements = new List<Vector3[]>();
        for(int childIndex = 0; childIndex < canvas.childCount; ++childIndex) {
            Vector3[] corners = new Vector3[4];
            canvas.GetChild(childIndex).gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
            cornersOfUIElements.Add(corners);
        }
    }

    void Update() {
        if (!gameManager.gamePaused) {
            for (int touchIndex = 0; touchIndex < Input.touchCount; ++touchIndex) {
                Touch currentTouch = Input.GetTouch(touchIndex);

                //ignores touch if it's position is over a ui element
                bool touchPositionOnUIElement = false;
                foreach (Vector3[] corners in cornersOfUIElements) {
                    if (currentTouch.position.x > corners[0].x &&
                        currentTouch.position.x < corners[3].x &&
                        currentTouch.position.y > corners[0].y &&
                        currentTouch.position.y < corners[1].y)
                        touchPositionOnUIElement = true;
                }

                bool touchPositionInDeadZone = currentTouch.position.x < deadZone.x && currentTouch.position.y < deadZone.y;

                if (currentTouch.phase == TouchPhase.Moved && !touchPositionOnUIElement && !touchPositionInDeadZone && joystick.GetAxis("Vertical") == 0 && joystick.GetAxis("Horizontal") == 0) {
                    float dX = currentTouch.deltaPosition.x / mainCamera.pixelWidth * 360;
                    transform.Rotate(Vector3.up * dX);
                    mainCamera.transform.RotateAround(transform.position, Vector3.up, dX);
                }
            }
        }
    }
}
