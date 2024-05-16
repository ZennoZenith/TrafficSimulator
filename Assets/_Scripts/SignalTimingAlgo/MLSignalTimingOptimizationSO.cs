using Simulator.SignalTiming;
using Simulator.TrafficSignal;
using System.Linq;
using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm/MLSignalTImingOptimization", fileName = "DefaultMLSignalTImingOptimization", order = 2)]
    internal class MLSignalTimingOptimizationSO : UnityEngine.ScriptableObject {

        public int REWARD_MULTIPLYER = 500;
        int lastTotalNumberOfVehicles = 0;
        float lastTime = 0;

        // phaseIndex + (4 incomming ares) * (15 vehicles per area) * ( distance + wait time) 
        private void CheckVehicles(IntersectionDataCalculator intersectionDataCalculator, ML_DATA ml_data) {
            Vector3 selfPosition = intersectionDataCalculator.transform.position;

            //Transform temp = incommingAreas[0];
            //numberOfHits = Physics.OverlapBoxNonAlloc(temp.position, temp.localScale / 2, hitColliders, Quaternion.identity, LayerMask.GetMask("Vehicle"));
            //for (int i = 0; i < NUM_OF_OBSERVATIONS_PER_VEHICLE; i++) {
            int observationIndex;
            for (int legIndex = 0; legIndex < ml_data.NUM_OF_LEGS; legIndex++) {
                observationIndex = 0;
                int numberOfVehicelsAtLegIndex = intersectionDataCalculator.vehiclesWaitingAtLeg[legIndex].Count;
                var list = intersectionDataCalculator.vehiclesWaitingAtLeg[legIndex].ToArray();
                for (int j = 0; j < ml_data.NUM_OF_VEHICLES_PER_LEG; j++) {
                    if (j < numberOfVehicelsAtLegIndex) {
                        ml_data.observations[(legIndex * observationIndex) + j + ml_data.OFSET] = Vector3.Distance(selfPosition, list[j].Key.transform.position);
                    }
                    else {
                        ml_data.observations[(legIndex * observationIndex) + j + ml_data.OFSET] = -1;
                    }
                }

                observationIndex = 1;
                for (int letIndex = 0; letIndex < ml_data.NUM_OF_LEGS; letIndex++) {
                    for (int j = 0; j < ml_data.NUM_OF_VEHICLES_PER_LEG; j++) {
                        if (j < numberOfVehicelsAtLegIndex) {
                            ml_data.observations[(legIndex * observationIndex) + j + ml_data.OFSET] = list[j].Value;
                        }
                        else {
                            ml_data.observations[(letIndex * observationIndex) + j + ml_data.OFSET] = -1;
                        }
                    }
                }

            }

        }

        //public (float[], float) CalculateRewards(IntersectionDataCalculator intersectionDataCalculator, ML_DATA ml_data) {
        public void CalculateRewards(IntersectionDataCalculator intersectionDataCalculator, ML_DATA ml_data) {
            float time = Time.time;
            float throughput = (intersectionDataCalculator.TotalNumberOfVehicles - lastTotalNumberOfVehicles) / (time - lastTime);
            lastTime = time;
            lastTotalNumberOfVehicles = intersectionDataCalculator.TotalNumberOfVehicles;
            CheckVehicles(intersectionDataCalculator, ml_data);

            float rewards = throughput * REWARD_MULTIPLYER;
            //float rewards = 0;

            int observationIndex = 1;
            for (int legIndex = 0; legIndex < ml_data.NUM_OF_LEGS; legIndex++) {
                for (int j = 0; j < ml_data.NUM_OF_VEHICLES_PER_LEG; j++) {
                    if (ml_data.observations[(legIndex * observationIndex) + j + ml_data.OFSET] >= 0) {
                        rewards -= ml_data.observations[(legIndex * observationIndex) + j + ml_data.OFSET];
                    }
                }
            }
            Debug.Log($"Reward function reward: {rewards}");
            ml_data.rewards = rewards;
            //return (ml_data.observations, rewards);
        }
    }
}
