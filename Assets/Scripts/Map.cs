using UnityEngine;

/// <summary> Makes map camera follow player </summary>
public class Map : MonoBehaviour {
    public Transform unityChan;

    void LateUpdate() {
        Vector3 newPos = unityChan.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, unityChan.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
