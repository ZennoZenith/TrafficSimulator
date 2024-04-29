using Simulator.RuntimeData;
using Simulator.ScriptableObject;
using UnityEngine;

namespace Simulator.ML {
    public class TraficSignalMLData : MonoBehaviour {
        [SerializeField] private MLAlgorithmSO mlAlgorithmSettings;

        [field: SerializeField] public Transform[] incommingAreas { get; private set; }
        //private int lastNumberOfVehicles = 0;
        //private float lastCheckedTime = 0;
        Vector3 selfPosition;

        const int OFSET = 1;

        const int NUM_OF_AREA = 4;
        const int NUM_OF_VEHICLES_PER_AREA = 15;
        const int NUM_OF_OBSERVATIONS_PER_VEHICLE = 2;


        readonly Collider[] hitColliders = new Collider[40];

        // phaseIndex + (4 incomming ares) * (15 vehicles per area) * ( distance + wait time) 
        private readonly float[] observations = new float[OFSET + (NUM_OF_AREA * NUM_OF_VEHICLES_PER_AREA * NUM_OF_OBSERVATIONS_PER_VEHICLE)];

        //private void Awake() {
        //    intersectionDataCalculator = GetComponent<IntersectionDataCalculator>();
        //}

        void Start() {
            selfPosition = transform.position;
        }

        int numberOfHits;

        VehicleDataCalculator vdc;
        private void CheckVehicles() {
            Transform temp = incommingAreas[0];
            numberOfHits = Physics.OverlapBoxNonAlloc(temp.position, temp.localScale / 2, hitColliders, Quaternion.identity, LayerMask.GetMask("Vehicle"));
            //for (int i = 0; i < NUM_OF_OBSERVATIONS_PER_VEHICLE; i++) {
            int observationIndex = 0;


            for (int areaIndex = 0; areaIndex < NUM_OF_AREA; areaIndex++) {
                for (int j = 0; j < NUM_OF_VEHICLES_PER_AREA; j++) {
                    if (j < numberOfHits) {
                        observations[(areaIndex * observationIndex) + j + OFSET] = Vector3.Distance(selfPosition, hitColliders[j].transform.position);
                    }
                    else {
                        observations[(areaIndex * observationIndex) + j + OFSET] = -1;
                    }
                }

            }

            observationIndex = 1;
            for (int areaIndex = 0; areaIndex < NUM_OF_AREA; areaIndex++) {
                for (int j = 0; j < NUM_OF_VEHICLES_PER_AREA; j++) {
                    if (j < numberOfHits) {
                        if (hitColliders[j].TryGetComponent(out vdc))
                            observations[(areaIndex * observationIndex) + j + OFSET] = vdc.WaitTimeBeforeReachingIntersesction;
                    }
                    else {
                        observations[(areaIndex * observationIndex) + j + OFSET] = -1;
                    }
                }
            }
        }

        private float CalculateRewards(float vehicleThroughput) {
            float rewards = vehicleThroughput * mlAlgorithmSettings.REWARD_MULTIPLYER1;

            int observationIndex = 1;
            for (int areaIndex = 0; areaIndex < NUM_OF_AREA; areaIndex++) {
                for (int j = 0; j < NUM_OF_VEHICLES_PER_AREA; j++) {
                    if (observations[(areaIndex * observationIndex) + j + OFSET] >= 0) {
                        rewards -= observations[(areaIndex * observationIndex) + j + OFSET];
                    }
                }
            }
            return rewards;

        }


        public (float[], float) GetObservationsAndRewards(int phaseIndex, float throughput) {
            // 0 index (OFSET = 1) is phase index
            observations[0] = phaseIndex;
            CheckVehicles();
            float reward = CalculateRewards(throughput);
            return (observations, reward);
        }


    }
}