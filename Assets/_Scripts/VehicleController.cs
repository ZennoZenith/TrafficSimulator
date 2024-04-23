using UnityEditor;
using UnityEngine;

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

    private bool isBrakePressed;
    private float speed;
    private float previousSpeed;
    private float acceleration;

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
        (InputVector, isBrakePressed) = vehicleDriverAI.CalculateAiInput();
        if (InputVector == Vector3.zero) return;

        Debug.Log(speed);

        ProcessInput();
    }

    float step;
    void ProcessInput() {
        //transform.forward
        CalculateSpeed();
        //transform.Translate(InputVector * speed);
        step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, InputVector, speed);
        //transform.Translate(Vector3.forward * Time.deltaTime);

    }

    void CalculateSpeed() {
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

        Gizmos.color = Color.black;
        Gizmos.DrawSphere(InputVector, 0.3f);
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
