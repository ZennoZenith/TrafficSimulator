using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(VehicleDriverAI))]
public class VehicleController : MonoBehaviour {
    [SerializeField] private VehicleDriverAI vehicleDriverAI;
    [SerializeField] private VehicleDataScriptableObject vehicleData;

    private void Awake() {
        if (vehicleDriverAI == null) {
            vehicleDriverAI = GetComponent<VehicleDriverAI>();
        }
    }

    public void Initialize() {

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
