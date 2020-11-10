using System;
using System.Collections;
using UnityEngine;

/// <summary> Animates islands </summary>
public class MoveIsland : MonoBehaviour {
    [HideInInspector] public bool moveIsland = true;

    [Serializable] public class Vertical {
        public bool enabled;
        [Serializable] public class VerticalKeyframes {
            public float yPos;
            public float time;
        }
        public VerticalKeyframes[] Keyframes = new VerticalKeyframes[3];
    }
    [Header("Types of Movement")]
    [Tooltip("Makes the island go up and down")]
    public Vertical VerticalMovement;

    [Serializable] public class Horizontal {
        public bool enabled;
        [Serializable] public class HorizontalKeyframes {
            public float xPos, zPos;
            public float time;
        }
        public HorizontalKeyframes[] Keyframes = new HorizontalKeyframes[3];
    }
    [Tooltip("Makes the island move on the X-Z plane")]
    public Horizontal HorizontalMovement;

    [Serializable] public class Radial {
        public bool enabled;
        public Vector3 about;    //pivot
        public Vector3 startPos; //initial position of island
        public Vector3 axis;     //axis
        [HideInInspector] public Vector3 normalizedAxis;  //normalized axis
        public enum Directions { anticlockwise, clockwise };
        public Directions direction = Directions.anticlockwise;    //direction of rotation
        public float period;     //time for one complete revolution, in seconds
    }
    [Tooltip("Makes the island spin round and round")]
    public Radial RadialMovement;

    // Start is called before the first frame update
    void Start() {
        if (VerticalMovement.enabled && VerticalMovement.Keyframes.Length >= 3)
            StartCoroutine(MoveVertical());
        if (HorizontalMovement.enabled && HorizontalMovement.Keyframes.Length >= 3)
            StartCoroutine(MoveHorizontal());
        if (RadialMovement.enabled)
            transform.position = RadialMovement.startPos;
        RadialMovement.normalizedAxis = RadialMovement.axis.normalized;
    }

    // Update is called once per frame
    void Update() {
        if (RadialMovement.enabled && moveIsland) {
            if (RadialMovement.about != RadialMovement.startPos) {
                Quaternion prevRot = transform.rotation;
                transform.RotateAround(RadialMovement.about, RadialMovement.normalizedAxis, (RadialMovement.direction == Radial.Directions.clockwise ? 1 : -1) / RadialMovement.period * 360 * Time.deltaTime);
                transform.rotation = prevRot;   //change position but keep direction/orientation
            }
            else {
                transform.Rotate(RadialMovement.normalizedAxis * (RadialMovement.direction == Radial.Directions.clockwise ? 1 : -1) / RadialMovement.period * 360 * Time.deltaTime);
            }
        }
    }

    IEnumerator MoveVertical() {
        int index = 1;
        //apply the position at the first keyframe
        transform.position = new Vector3(transform.position.x, VerticalMovement.Keyframes[0].yPos, transform.position.z);

        while (index < VerticalMovement.Keyframes.Length) {
            //get time difference between this keyframe and the previous one
            float duration = VerticalMovement.Keyframes[index].time - VerticalMovement.Keyframes[index-1].time;

            //start movement to the position specified for this keyframe
            yield return StartCoroutine(LerpVertical(VerticalMovement.Keyframes[index - 1].yPos, VerticalMovement.Keyframes[index].yPos, duration));

            //move to next keyframe
            ++index;
            if (index == VerticalMovement.Keyframes.Length)
                index = 1;
        }
    }

    IEnumerator LerpVertical(float y0, float y1, float dTime) {
        float timeElapsed = 0f;
        while (timeElapsed < dTime) {
            Vector3 pos = transform.position, from = pos, to = pos;
            from.y = y0;
            to.y = y1;
            if (moveIsland)
                transform.position = Vector3.Lerp(from, to, timeElapsed / dTime);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator MoveHorizontal() {
        int index = 1;
        //apply the position at the first keyframe
        transform.position = new Vector3(HorizontalMovement.Keyframes[0].xPos, transform.position.y, HorizontalMovement.Keyframes[0].zPos);

        while (index < HorizontalMovement.Keyframes.Length) {
            //get time difference between this keyframe and the previous one
            float duration = HorizontalMovement.Keyframes[index].time - HorizontalMovement.Keyframes[index - 1].time;

            //start movement to the position specified for this keyframe
            yield return StartCoroutine(LerpHorizontal(HorizontalMovement.Keyframes[index - 1].xPos, HorizontalMovement.Keyframes[index].xPos, HorizontalMovement.Keyframes[index - 1].zPos, HorizontalMovement.Keyframes[index].zPos, duration));

            //move to next keyframe
            ++index;
            if (index == HorizontalMovement.Keyframes.Length)
                index = 1;
        }
    }

    IEnumerator LerpHorizontal(float x0, float x1, float z0, float z1, float dTime) {
        float timeElapsed = 0f;
        while (timeElapsed < dTime) {
            Vector3 pos = transform.position, from = pos, to = pos;
            from.x = x0;
            from.z = z0;
            to.x = x1;
            to.z = z1;
            if (moveIsland)
                transform.position = Vector3.Lerp(from, to, timeElapsed / dTime);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
