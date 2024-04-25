using UnityEngine;

public class FloatingText : MonoBehaviour {
    Transform mainCam;
    Transform unit;
    Transform worldSpaceCanvas;

    [SerializeField] private Vector3 offset;

    void Start() {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        worldSpaceCanvas = FindAnyObjectByType<Canvas>().transform;

        transform.SetParent(worldSpaceCanvas);
    }
    void Update() {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position); // Look at the camera
        transform.position = unit.position + offset;
    }
}
