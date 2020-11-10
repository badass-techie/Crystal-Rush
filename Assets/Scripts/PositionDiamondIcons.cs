using UnityEngine;

/// <summary> Positions diamond icons on minimap </summary>
public class PositionDiamondIcons : MonoBehaviour {
    public Transform unityChan;
    Transform gem;
    const float heightOffset = 10f;

    // Start is called before the first frame update
    void Start() {
        gem = GameObject.Find(gameObject.name.Split(' ')[0]).transform;
    }

    // Update is called once per frame
    void Update() {
        transform.position = gem.position + Vector3.up * heightOffset;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, unityChan.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
