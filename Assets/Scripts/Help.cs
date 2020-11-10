using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Controls the help scene </summary>
public class Help : MonoBehaviour {
    int state = 0;

    public Transform player;

    public Transform HUD;
    Text caption;
    Image textBackground;

    Camera mainCamera;

    RectTransform joystick, joystickHandle;
    float handleMovementRange;
    const float handleCoeff = 0.667f;
    Vector3 initialJoystickPosition;
    Vector3 touchStart;

    Vector3[] panBounds = new Vector3[4], moveBounds = new Vector3[4];
    bool panningHandled = false, movementHandled = false;

    // Start is called before the first frame update
    void Start() {
        PlayerPrefs.SetInt("Run", PlayerPrefs.GetInt("Run", 0) + 1);

        mainCamera = Camera.main;
        caption = HUD.Find("Text Background").Find("Text").gameObject.GetComponent<Text>();
        textBackground = HUD.Find("Text Background").gameObject.GetComponent<Image>();
        
        //gets bounds of panning and movement regions
        HUD.Find("Pan Area").gameObject.GetComponent<RectTransform>().GetWorldCorners(panBounds);
        HUD.Find("Move Area").gameObject.GetComponent<RectTransform>().GetWorldCorners(moveBounds);

        joystick = HUD.Find("Joystick").gameObject.GetComponent<RectTransform>();
        joystickHandle = joystick.GetChild(0).gameObject.GetComponent<RectTransform>();
        Vector3[] joystickRectTransformCorners = new Vector3[4];
        joystick.GetWorldCorners(joystickRectTransformCorners);
        handleMovementRange = handleCoeff * (joystickRectTransformCorners[3].x - joystickRectTransformCorners[0].x) / 2;
        initialJoystickPosition = joystick.position;

        StartCoroutine(ChangeState());
    }

    // Update is called once per frame
    void Update() {
        if (state == 7)
            HandlePanning();

        if (state == 8)
            HandleMovement();
    }

    IEnumerator ChangeState() {
        ++state;    //1
        caption.text = "Rina has lost the treasures of the sky's fairies\nand has to find them in time\nbefore they make her city float away.";
        yield return StartCoroutine(ZoomText(6f, caption.gameObject.GetComponent<Transform>(), new Vector3(0.7f, 0.7f, 0.7f), Vector3.one));

        ++state;    //2
        caption.text = "Help Rina recover the lost treasures.";
        yield return StartCoroutine(ZoomText(6f, caption.gameObject.GetComponent<Transform>(), new Vector3(0.7f, 0.7f, 0.7f), Vector3.one));

        ++state;    //3
        caption.text = "Jump from one island to another and\ncollect all crystals\nbefore the time runs out.";
        yield return StartCoroutine(ZoomText(6f, caption.gameObject.GetComponent<Transform>(), new Vector3(0.7f, 0.7f, 0.7f), Vector3.one));

        ++state;    //4
        caption.text = "";
        textBackground.CrossFadeAlpha(0f, 1f, true);
        yield return new WaitForSeconds(1f);

        ++state;    //5
        textBackground.gameObject.SetActive(false);
        HUD.Find("Jump Button").gameObject.SetActive(true);
        HUD.Find("Jump Text").gameObject.SetActive(true);
    }

    IEnumerator ZoomText(float duration, Transform text, Vector3 initialScale, Vector3 finalScale) {
        float timer = 0f;
        while(timer < duration) {
            text.localScale = Vector3.Lerp(initialScale, finalScale, timer/duration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void HandleAction(string actionName) {
        StartCoroutine(actionName);
    }

    IEnumerator Jump() {
        yield return new WaitForSeconds(0.2f);
        HUD.Find("Jump Button").gameObject.SetActive(false);
        HUD.Find("Jump Text").gameObject.SetActive(false);
        HUD.Find("Jump Button").gameObject.GetComponent<UIButtonController>().SetButtonDown(false);
        yield return new WaitForSeconds(4f);

        ++state;    //6
        HUD.Find("Flip Camera Button").gameObject.SetActive(true);
        HUD.Find("Flip Camera Text").gameObject.SetActive(true);
    }

    IEnumerator FlipCamera() {
        yield return new WaitForSeconds(2f);
        HUD.Find("Flip Camera Button").gameObject.SetActive(false);
        HUD.Find("Flip Camera Text").gameObject.SetActive(false);
        HUD.Find("Flip Camera Button").gameObject.GetComponent<UIButtonController>().SetButtonDown(false);
        yield return new WaitForSeconds(1f);

        ++state;    //7
        HUD.Find("Pan Icon").gameObject.SetActive(true);
        HUD.Find("Pan Text").gameObject.SetActive(true);
    }

    void HandlePanning() {
        for (int touchIndex = 0; touchIndex < Input.touchCount; ++touchIndex) {
            Touch currentTouch = Input.GetTouch(touchIndex);
            bool touchInRegion = currentTouch.position.x > panBounds[0].x &&
                                 currentTouch.position.x < panBounds[3].x &&
                                 currentTouch.position.y > panBounds[0].y &&
                                 currentTouch.position.y < panBounds[1].y;

            if (currentTouch.phase == TouchPhase.Moved && touchInRegion) {
                if (!panningHandled) {
                    panningHandled = true;
                    StartCoroutine(DisablePanning());
                }

                float dX = currentTouch.deltaPosition.x / mainCamera.pixelWidth * 360;
                player.Rotate(Vector3.up * dX);
                mainCamera.transform.RotateAround(transform.position, Vector3.up, dX);
            }
        }

        if (!panningHandled && Input.GetKeyDown(KeyCode.Escape)) {
            panningHandled = true;
            StartCoroutine(DisablePanning());
        }
    }

    IEnumerator DisablePanning() {
        yield return new WaitForSeconds(2f);

        HUD.Find("Pan Icon").gameObject.SetActive(false);
        HUD.Find("Pan Text").gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        ++state;    //8
        HUD.Find("Joystick").gameObject.SetActive(true);
        HUD.Find("Move Icon").gameObject.SetActive(true);
        HUD.Find("Move Text").gameObject.SetActive(true);
    }

    void HandleMovement() {
        for (int touchIndex = 0; touchIndex < Input.touchCount; ++touchIndex) {
            Touch currentTouch = Input.GetTouch(touchIndex);
            bool touchInRegion = currentTouch.position.x > moveBounds[0].x &&
                                 currentTouch.position.x < moveBounds[3].x &&
                                 currentTouch.position.y > moveBounds[0].y &&
                                 currentTouch.position.y < moveBounds[1].y;

            // Check each touch's phase
            switch (currentTouch.phase) {
                case TouchPhase.Began:
                    if (touchInRegion) {
                        touchStart = currentTouch.position;
                        joystick.position = touchStart;
                        joystickHandle.localPosition = Vector3.zero;
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    joystick.position = initialJoystickPosition;
                    joystickHandle.localPosition = Vector3.zero;
                    break;
                case TouchPhase.Moved:
                    bool touchStartInRegion = touchStart.x > moveBounds[0].x &&
                                              touchStart.x < moveBounds[3].x &&
                                              touchStart.y > moveBounds[0].y &&
                                              touchStart.y < moveBounds[1].y;
                    if (touchStartInRegion) {
                        if (!movementHandled) {
                            movementHandled = true;
                            StartCoroutine(DisableMovement());
                        }

                        Vector3 currentTouchPosition = new Vector3(currentTouch.position.x, currentTouch.position.y);
                        Vector3 deltaSinceTouchStart = currentTouchPosition - touchStart;
                        if (deltaSinceTouchStart.magnitude < handleMovementRange)
                            joystickHandle.position += new Vector3(currentTouch.deltaPosition.x, currentTouch.deltaPosition.y);
                        else
                            joystickHandle.localPosition = (currentTouchPosition - joystick.position).normalized * handleMovementRange;
                    }
                    break;
            }
        }

        if (!movementHandled && Input.GetKeyDown(KeyCode.Escape)) {
            movementHandled = true;
            StartCoroutine(DisableMovement());
        }
    }

    public float GetAxis(string axis) {
        if (axis == "Horizontal")
            return joystickHandle.localPosition.x / handleMovementRange;
        else if (axis == "Vertical")
            return joystickHandle.localPosition.y / handleMovementRange;
        else
            return 0f;
    }

    IEnumerator DisableMovement() {
        yield return new WaitForSeconds(2f);

        joystick.position = initialJoystickPosition;
        joystickHandle.localPosition = Vector3.zero;
        HUD.Find("Joystick").gameObject.SetActive(false);
        HUD.Find("Move Icon").gameObject.SetActive(false);
        HUD.Find("Move Text").gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        ++state;    //9
        HUD.Find("Minimap").Find("Text").gameObject.SetActive(true);
    }

    IEnumerator ShowComplete() {
        caption.text = "All set!";
        caption.fontSize = 120;
        textBackground.gameObject.SetActive(true);
        textBackground.color = new Color(textBackground.color.r, textBackground.color.g, textBackground.color.b, 1f);
        textBackground.CrossFadeAlpha(0f, 0f, true);
        textBackground.CrossFadeAlpha(1f, 3f, true);
        yield return StartCoroutine(ZoomText(3f, caption.gameObject.GetComponent<Transform>(), new Vector3(0.7f, 0.7f, 0.7f), Vector3.one));
        Close();
    }

    void Close() {
        //if this is the first time this scene is running
        if (PlayerPrefs.GetInt("Run", 0) <= 1) {
            int recentLevel = PlayerPrefs.GetInt("RecentLevel");
            SceneManager.LoadScene("Level " + recentLevel);
        }
        else {
            SceneManager.LoadScene("Welcome");
        }
    }

    public void Back() {
        if (FindObjectOfType<AudioManager>() != null && Convert.ToBoolean(PlayerPrefs.GetInt("IsMuted", 0)) == false)
            FindObjectOfType<AudioManager>().Play("Click");
        SceneManager.LoadScene("Welcome");
    }
}
