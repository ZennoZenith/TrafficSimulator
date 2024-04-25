using System.Collections;
using UnityEngine;

[RequireComponent(typeof(VehicleController), typeof(VehicleDriverAI))]
public class VehicleDataCalculator : MonoBehaviour {
    [SerializeField] private GameSettingsScriptableObject gameSettings;

    private VehicleController vehicleController;
    private VehicleDriverAI vehicleDriverAI;
    private bool _reachedAnIntersection;

    public bool Initialized { get; private set; } = false;
    public int TotalWaitTime { get; private set; }
    public float TotalDistanceTraveled { get; private set; }
    public int TotalTimeTaken { get; private set; }

    public int WaitTimeBeforeReachingIntersesction { get; private set; }
    public bool ReachedAnIntersection {
        get { return _reachedAnIntersection; }
        private set {
            if (_reachedAnIntersection == true && value == false) {
                StoreData.WriteIntesectionWaitTime(intersectionRoadSetup.name, transform.name, WaitTimeBeforeReachingIntersesction);
                WaitTimeBeforeReachingIntersesction = 0;
            }
            _reachedAnIntersection = value;
        }
    }


    private RoadSetup intersectionRoadSetup;
    private Coroutine tickCoroutine;

    private void Awake() {
        vehicleController = GetComponent<VehicleController>();
        vehicleDriverAI = GetComponent<VehicleDriverAI>();
    }

    void OnEnable() {
        Initialize();
    }

    void OnDisable() {
        DeInitialize();
    }

    IEnumerator Tick() {
        while (true) {
            yield return new WaitForSeconds(1f);

            if (vehicleController.Speed < gameSettings.considerStopSpeed) {
                TotalWaitTime++;
                WaitTimeBeforeReachingIntersesction++;
            }

            TotalTimeTaken++;

            if (TotalDistanceTraveled == 0 && vehicleDriverAI.Initialized) {
                TotalDistanceTraveled = vehicleDriverAI.DistanceToTravel();
            }


            var currentRoadSetup = vehicleDriverAI.ShortestPathNodes[vehicleDriverAI.CurrentNodeIndex];

            if (currentRoadSetup.RoadTypeSO.isIntersection) {
                ReachedAnIntersection = true;
                intersectionRoadSetup = currentRoadSetup;
            }
            else
                ReachedAnIntersection = false;

        }
    }

    private void Initialize() {
        _reachedAnIntersection = false;
        TotalWaitTime = WaitTimeBeforeReachingIntersesction = 0;
        TotalTimeTaken = 0;
        TotalDistanceTraveled = 0;

        ReachedAnIntersection = false;
        tickCoroutine = StartCoroutine(Tick());
        Initialized = true;
    }

    private void DeInitialize() {
        StopCoroutine(tickCoroutine);
        Initialized = false;
        StoreData.WriteVehicleRuntimeData(new VehicleRuntimeData {
            vehicleName = transform.name,
            TotalDistanceTraveled = TotalDistanceTraveled,
            TotalTimeTaken = TotalTimeTaken,
            TotalWaitTime = TotalWaitTime

        });
    }


}
