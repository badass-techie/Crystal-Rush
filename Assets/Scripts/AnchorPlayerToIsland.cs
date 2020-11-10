using UnityEngine;

/// <summary> Makes Unity Chan and her camera move along with the island she's on top of </summary>
public class AnchorPlayerToIsland : MonoBehaviour {
    public Transform unityChan;
    [HideInInspector] public Transform lastIslandInContact;
    [HideInInspector] public float distanceInAir;
    float heightAtLastContact;

    // Update is called once per frame
    void Update() {
        transform.position = unityChan.position;
        if (transform.parent.parent == null) { //if airborne
            distanceInAir = unityChan.position.y - heightAtLastContact;
        }
        else {
            distanceInAir = 0;
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.name != "Unity Chan") {
            lastIslandInContact = other.transform.root;
            transform.parent.parent = other.transform.root;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.name != "Unity Chan") {
            transform.parent.parent = null;
            heightAtLastContact = unityChan.position.y;
        }
    }
}
