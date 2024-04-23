using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(VehicleDriverAI))]
public class VehicleController : MonoBehaviour {
    [SerializeField] private VehicleDriverAI vehicleDriverAI;
    [SerializeField] private VehicleDataScriptableObject vehicleData;
    [SerializeField] private Vector3 targetPosition;

    private List<Vector3> pointsToFollow;
    private Vector3 inputVector;
    private bool isBrakePressed;

    public bool Initialized { get; private set; } = false;

    private void Awake() {
        if (vehicleDriverAI == null) {
            vehicleDriverAI = GetComponent<VehicleDriverAI>();
        }
    }

    public void Initialize() {

        pointsToFollow = vehicleDriverAI.PointsToFollow;
        Initialized = true;
    }

    public void DeInitialize() {

        Initialized = false;
    }

    private void FixedUpdate() {
        if (!Initialized) return;
        (inputVector, isBrakePressed) = vehicleDriverAI.CalculateAiInput();

        ProcessInput();


    }

    private void ProcessInput() {
    }

}


# if UNITY_EDITOR
[CustomEditor(typeof(VehicleController))]
public class VehicleControllerEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        VehicleController vehicleController = (VehicleController)target;

        if (GUILayout.Button("Initialize")) {
            vehicleController.Initialize();
        }

    }
}

#endif
