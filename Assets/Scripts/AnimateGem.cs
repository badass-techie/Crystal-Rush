using UnityEngine;

/// <summary> Animates gems </summary>
public class AnimateGem : MonoBehaviour {
    [Tooltip("Whether to animate this gem")]
    public bool isAnimated = true;

    [Header("Rotation")]
    [Tooltip("Whether to rotate")]
    public bool isRotating = false;
    [Tooltip("Time for one complete revolution")]
    public float period;

    [Header("Levitation")]
    [Tooltip("Whether to float")]
    public bool isFloating = false;
    [Tooltip("Distance covered when going up")]
    public float floatDistance;
    [Tooltip("Time it takes to go up")]
    public float floatDuration;
    private float floatTimer;
    private bool goingUp = true;

    [Header("Scaling")]
    [Tooltip("Whether to adjust scale")]
    public bool isScaling = false;
    [Tooltip("Start scale")]
    public Vector3 startScale;
    [Tooltip("End scale")]
    public Vector3 endScale;
    [Tooltip("Time from start to end scale")]
    public float scaleDuration;
    private bool scalingUp = true;
    private float scaleTimer;
    private Vector3 childScale;

    void Start() {
        //startScale = new Vector3(startScale.x / transform.parent.localScale.x, startScale.y / transform.parent.localScale.y, startScale.z / transform.parent.localScale.z);
        //endScale = new Vector3(endScale.x / transform.parent.localScale.x, endScale.y / transform.parent.localScale.y, endScale.z / transform.parent.localScale.z);
        if (isAnimated && isScaling) {
            //childScale = transform.GetChild(0).localScale;
            transform.localScale = startScale;
        }
    }
    void FixedUpdate() {
        if (isAnimated) {
            if (isRotating) {
                transform.Rotate(Vector3.up * (360 * Time.deltaTime / period));
            }

            if (isFloating) {
                floatTimer += Time.fixedDeltaTime;
                Vector3 moveDir = Vector3.up * floatDistance*Time.fixedDeltaTime/floatDuration;
                transform.Translate(moveDir);

                if (goingUp && floatTimer >= floatDuration) {
                    goingUp = false;
                    floatTimer = 0;
                    floatDistance = -floatDistance;
                }
                else if (!goingUp && floatTimer >= floatDuration) {
                    goingUp = true;
                    floatTimer = 0;
                    floatDistance = +floatDistance;
                }
            }

            if (isScaling) {
                scaleTimer += Time.fixedDeltaTime;

                if (scalingUp)
                    transform.localScale = Vector3.Lerp(transform.localScale, endScale, Time.fixedDeltaTime/scaleDuration);
                else
                    transform.localScale = Vector3.Lerp(transform.localScale, startScale, Time.fixedDeltaTime / scaleDuration);

                if (scaleTimer >= scaleDuration) {
                    scalingUp = !scalingUp;
                    scaleTimer = 0;
                }

                //if (transform.GetChild(0) != null)
                    //transform.GetChild(0).localScale = 3.5f * new Vector3(childScale.x / transform.localScale.x, childScale.y / transform.localScale.y, childScale.z / transform.localScale.z); ;
            }
        }
    }
}
