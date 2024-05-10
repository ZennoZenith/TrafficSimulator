using Simulator.SignalTiming;
using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm/MLSignalTImingOptimization", fileName = "DefaultMLSignalTImingOptimization", order = 2)]
    internal class MLSignalTimingOptimizationSO : UnityEngine.ScriptableObject, ISignalTimingAlgorithm {

        // 0 index (OFSET = 1) is phase index
        public int OFSET = 1;
        public int NUM_OF_LEGS = 4;
        public int NUM_OF_VEHICLES_PER_LEGS = 15;
        public int NUM_OF_OBSERVATIONS_PER_VEHICLE = 2;
        public int REWARD_MULTIPLYER = 500;

        public (int, float) GetNextPhase(TrafficLightSetup trafficLightSetup) {


            throw new System.NotImplementedException();
        }

        [field: SerializeField] public Transform[] incommingAreas { get; private set; }
        //private int lastNumberOfVehicles = 0;
        //private float lastCheckedTime = 0;
        //Vector3 selfPosition;
        readonly Collider[] hitColliders = new Collider[40];

        // phaseIndex + (4 incomming ares) * (15 vehicles per area) * ( distance + wait time) 
        private float[] observations;
        private void OnEnable() {
            observations = new float[OFSET + (NUM_OF_LEGS * NUM_OF_VEHICLES_PER_LEGS * NUM_OF_OBSERVATIONS_PER_VEHICLE)];
        }

        int numberOfHits;

        //VehicleDataCalculator vdc;
        private void CheckVehicles(Vector3 selfPosition) {
            Transform temp = incommingAreas[0];
            numberOfHits = Physics.OverlapBoxNonAlloc(temp.position, temp.localScale / 2, hitColliders, Quaternion.identity, LayerMask.GetMask("Vehicle"));
            //for (int i = 0; i < NUM_OF_OBSERVATIONS_PER_VEHICLE; i++) {
            int observationIndex = 0;


            for (int areaIndex = 0; areaIndex < NUM_OF_LEGS; areaIndex++) {
                for (int j = 0; j < NUM_OF_VEHICLES_PER_LEGS; j++) {
                    if (j < numberOfHits) {
                        observations[(areaIndex * observationIndex) + j + OFSET] = Vector3.Distance(selfPosition, hitColliders[j].transform.position);
                    }
                    else {
                        observations[(areaIndex * observationIndex) + j + OFSET] = -1;
                    }
                }

            }

            observationIndex = 1;
            for (int areaIndex = 0; areaIndex < NUM_OF_LEGS; areaIndex++) {
                for (int j = 0; j < NUM_OF_VEHICLES_PER_LEGS; j++) {
                    if (j < numberOfHits) {
                        //if (hitColliders[j].TryGetComponent(out vdc))
                        //observations[(areaIndex * observationIndex) + j + OFSET] = vdc.WaitTimeBeforeReachingIntersesction;
                    }
                    else {
                        observations[(areaIndex * observationIndex) + j + OFSET] = -1;
                    }
                }
            }
        }

        private float CalculateRewards(float vehicleThroughput) {
            float rewards = vehicleThroughput * REWARD_MULTIPLYER;

            int observationIndex = 1;
            for (int areaIndex = 0; areaIndex < NUM_OF_LEGS; areaIndex++) {
                for (int j = 0; j < NUM_OF_VEHICLES_PER_LEGS; j++) {
                    if (observations[(areaIndex * observationIndex) + j + OFSET] >= 0) {
                        rewards -= observations[(areaIndex * observationIndex) + j + OFSET];
                    }
                }
            }
            return rewards;

        }
    }
}
