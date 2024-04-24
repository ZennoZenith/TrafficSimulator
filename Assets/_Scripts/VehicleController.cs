using System;
using UnityEditor;
using UnityEngine;

public enum BrakeState {
    NoBrake,
    Brake,
    HandBrake,
    EmergencyBrake
}

[RequireComponent(typeof(VehicleDriverAI))]
public class VehicleController : MonoBehaviour {
    [SerializeField] private VehicleDriverAI vehicleDriverAI;
    [SerializeField] private VehicleDataScriptableObject vehicleData;
    [SerializeField] private Vector3 targetPosition;

    //private List<Vector3> pointsToFollow;
    //private Vector3 _inputVector;
    //private Vector3 InputVector {
    //    set {
    //        _inputVector = value.normalized;
    //    }
    //    get { return _inputVector; }
    //}

    public Vector3 InputVector { get; private set; }

    [SerializeField] private BrakeState brakeState = BrakeState.NoBrake;
    [SerializeField] private float speed;
    [SerializeField] private float targetSpeed;
    private float acceleration;
    Vector3 forward;

    public bool Initialized { get; private set; } = false;

    private void Awake() {
        if (vehicleDriverAI == null) {
            vehicleDriverAI = GetComponent<VehicleDriverAI>();
        }
    }

    private void Start() {
        Initialize();

    }

    public void Initialize() {
        vehicleDriverAI.Initialize();
        //pointsToFollow = vehicleDriverAI.PointsToFollow;
        Initialized = true;
    }

    public void DeInitialize() {

        Initialized = false;
    }

    private void FixedUpdate() {
        //transform.position = Vector3.MoveTowards(transform.position, Vector3.forward, 0.5f);
        //transform.Translate(Vector3.forward * Time.fixedDeltaTime);

        if (!Initialized) return;
        //(InputVector, isBrakePressed) = vehicleDriverAI.GetNextPointToFollow();

        (InputVector, targetPosition, targetSpeed, brakeState) = vehicleDriverAI.CalculateAiInput();
        if (InputVector == Vector3.zero) return;

        if (speed > targetSpeed && brakeState == BrakeState.NoBrake)
            brakeState = BrakeState.Brake;

        ProcessInput();
        ProcessRotation();

        brakeState = BrakeState.NoBrake;
    }

    private void ProcessRotation() {
        Vector3 direction = targetPosition - transform.position;
        Quaternion rotGoal = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, vehicleData.turnSpeed);
    }

    //float step;
    private void ProcessInput() {
        //transform.forward
        CalculateSpeed();
        //Debug.Log(speed);
        forward = transform.InverseTransformDirection(InputVector);
        transform.Translate(speed * Time.fixedDeltaTime * forward);
    }

    private void HandleBrake() {
        if (brakeState == BrakeState.Brake) {
            if (Mathf.Abs(speed) < 0.1f)
                speed = 0;
            else
                speed -= Mathf.Sign(speed) * vehicleData.brakeAcceleration * Time.fixedDeltaTime;
            return;
        }
    }

    void CalculateSpeed() {
        if (brakeState != BrakeState.NoBrake) {
            HandleBrake();
            return;
        }
        acceleration = vehicleData.maxAcceleration * vehicleData.accelerationCurve.Evaluate(Utils.InverseLerpUnclamped(0, vehicleData.maxSpeed, speed));
        speed += acceleration * Time.fixedDeltaTime;
    }

    #region  Debug
    public float debugAcceleration;

    [Range(-10f, 30f)]
    public float debugSpeed;
    private void OnDrawGizmosSelected() {
        debugAcceleration = vehicleData.accelerationCurve.Evaluate(Utils.InverseLerpUnclamped(0, vehicleData.maxSpeed, debugSpeed));
        //debugAcceleration = Utils.InverseLerpUnclamped(0, vehicleData.maxSpeed, debugSpeed);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, InputVector * 10);
    }
    #endregion

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
