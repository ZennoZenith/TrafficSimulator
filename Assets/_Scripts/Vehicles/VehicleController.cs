using Simulator.AI;
using Simulator.Manager;
using Simulator.ScriptableObject;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Simulator.Vehicle {
    public enum BrakeState {
        NoBrake,
        Brake,
        HandBrake,
        EmergencyBrake
    }

    [RequireComponent(typeof(VehicleDriverAI))]
    public class VehicleController : MonoBehaviour {
        [SerializeField] private UIManagerSO UIManager;
        [SerializeField] private TextMeshProUGUI speedTextUI;
        [field: SerializeField] public VehicleDriverAI VehicleDriverAI { get; private set; }
        [SerializeField] private VehicleDataSO vehicleData;
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
        public float Speed { get; private set; }
        [SerializeField] private float targetSpeed;
        private float acceleration;
        Vector3 forward;

        public bool Initialized { get; private set; } = false;

        private void Awake() {
            if (VehicleDriverAI == null) {
                VehicleDriverAI = GetComponent<VehicleDriverAI>();
            }
        }

        private void Start() {
            Initialize();
        }

        public void Initialize() {
            //vehicleDriverAI.Initialize();
            //pointsToFollow = vehicleDriverAI.PointsToFollow;
            Initialized = true;
        }

        public void DeInitialize() {
            Initialized = false;
            //gameObject.SetActive(false);
            ObjectPoolManager.ReturnObjectToPool(gameObject);
        }

        private void FixedUpdate() {
            //transform.position = Vector3.MoveTowards(transform.position, Vector3.forward, 0.5f);
            //transform.Translate(Vector3.forward * Time.fixedDeltaTime);

            if (!Initialized) return;
            //(InputVector, isBrakePressed) = vehicleDriverAI.GetNextPointToFollow();

            (InputVector, targetPosition, targetSpeed, brakeState) = VehicleDriverAI.CalculateAiInput();
            if (InputVector == Vector3.zero) return;

            if (Speed > targetSpeed && brakeState == BrakeState.NoBrake)
                brakeState = BrakeState.Brake;

            ProcessInput();
            ProcessRotation();

            brakeState = BrakeState.NoBrake;

            if (speedTextUI)
                speedTextUI.text = Math.Round(Speed, 2).ToString();
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
            transform.Translate(Speed * Time.fixedDeltaTime * forward);
        }

        private void HandleBrake() {
            //if (Mathf.Abs(Speed) < 0.5f) {
            // If Abs is not in use means that vehicle cannot have negative speed;
            if (Speed < 0.1f) {
                Speed = 0;
                return;
            }
            if (brakeState == BrakeState.Brake) {
                Speed -= Mathf.Sign(Speed) * vehicleData.brakeAcceleration * Time.fixedDeltaTime;
                return;
            }
            if (brakeState == BrakeState.HandBrake) {
                Speed -= Mathf.Sign(Speed) * vehicleData.brakeAcceleration * 10 * Time.fixedDeltaTime;
                return;
            }
            if (brakeState == BrakeState.EmergencyBrake) {
                Speed = 0;
                return;
            }
        }

        void CalculateSpeed() {
            if (brakeState != BrakeState.NoBrake) {
                HandleBrake();
                return;
            }
            // Supposing speed cannot be negative
            if (Speed < 0f) {
                Speed = 0;
                return;
            }
            acceleration = vehicleData.maxAcceleration * vehicleData.accelerationCurve.Evaluate(CoreUtils.InverseLerpUnclamped(0, vehicleData.maxSpeed, Speed));
            Speed += acceleration * Time.fixedDeltaTime;
        }


        #region  Debug
        #endregion

    }


#if UNITY_EDITOR
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
}