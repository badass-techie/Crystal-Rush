using UnityEngine;

/// <summary> Positions home icon on minimap </summary>
public class PositionHomeIcon : MonoBehaviour {
    public Transform unityChan;
    public float minimapRadius = 4;
    Vector3 defaultPos;

    // Start is called before the first frame update
    void Start() {
        defaultPos = transform.position;
    }
    // Update is called once per frame
    void Update() {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, unityChan.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        if (new Vector2(unityChan.position.x, unityChan.position.z).sqrMagnitude > minimapRadius * minimapRadius) {
            Vector3 vector = unityChan.position;
            vector.y = transform.position.y;
            float distanceToMove = vector.magnitude - minimapRadius;
            Vector3 newPos = vector.normalized * distanceToMove;
            newPos.y = defaultPos.y;
            transform.position = newPos;
        }
        else {
            transform.position = defaultPos;
        }
    }
}
