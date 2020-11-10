using UnityEngine;

/// <summary> Makes minimap camera follow player </summary>
public class Minimap : MonoBehaviour {
    public Transform unityChan;

    void LateUpdate() {
        Vector3 newPos = unityChan.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, unityChan.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
