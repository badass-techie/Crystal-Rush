using UnityEngine;

public class RotateCompass : MonoBehaviour {
    public Transform unityChan;
    
    // Update is called once per frame
    void Update() {
        transform.rotation = Quaternion.Euler(0, 0, unityChan.rotation.eulerAngles.y);
    }
}
