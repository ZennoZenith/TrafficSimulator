using Simulator.TrafficSignal;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Simulator.SignalTiming {
    [RequireComponent(typeof(TrafficLightSetup))]
    public class MLSignalOptimizationAlgorithm : Agent, ISignalTimingAlgorithm {
        private float action;

        private TrafficLightSetup trafficLightSetup;
        private float greenLightTime;
        private Phase[] phases;

        protected override void Awake() {
            Academy.Instance.AutomaticSteppingEnabled = false;
            base.Awake();
            trafficLightSetup = GetComponent<TrafficLightSetup>();
            phases = trafficLightSetup.Phases;
        }


        private void ChangeToNextPhaseWithTimeInterpolate(float time) {
            int index = (trafficLightSetup.CurrentPhaseIndex + 1) % phases.Length;
            //greenLightTime = Mathf.FloorToInt(Mathf.Lerp(phases[index].minGreenLightTime, phases[index].maxGreenLightTime, (time + 1) / 2));
            greenLightTime = Mathf.FloorToInt(Mathf.Lerp(phases[index].greenLightTime - 10, phases[index].greenLightTime + 10, (time + 1) / 2));
        }

        const int OFSET = 1;

        private const int NUM_OF_LEGS = 4;
        private const int NUM_OF_VEHICLES_PER_LEG = 15;
        private const int NUM_OF_OBSERVATIONS_PER_VEHICLE = 2;
        private const int REWARD_MULTIPLYER1 = 2000;
        private readonly float[] observations = new float[OFSET + (NUM_OF_LEGS * NUM_OF_VEHICLES_PER_LEG * NUM_OF_OBSERVATIONS_PER_VEHICLE)];

        //private float GenerateAction(float reward, float[] obseve) {
        //    AddReward(reward);
        //    //Debug.Log(GetCumulativeReward());
        //    EndEpisode();

        //    print("Decision requested");
        //    Academy.Instance.EnvironmentStep();
        //    print("Decision complete");


        //    //BeginEpisode(); 


        //    //observations = obseve.ToList();

        //    //float sum = 0;
        //    //foreach (var action in ActionReceiveBuffer) {
        //    //    sum += action;
        //    //}
        //    ////print(sum);

        //    //return sum / ACTION_BUFFER_SIZE;
        //}



        private float CalculateReward() {
            //int numberOfVehicles = intersectionDataCalculator.TotalNumberOfVehicles;
            //float throughput = (numberOfVehicles - lastNumberOfVehicles) / (Time.time - lastCheckedTime);
            //lastNumberOfVehicles = numberOfVehicles;

            //float rewards = vehicleThroughput * REWARD_MULTIPLYER1;

            //int observationIndex = 1;
            //for (int areaIndex = 0; areaIndex < NUM_OF_LEGS; areaIndex++) {
            //    for (int j = 0; j < NUM_OF_VEHICLES_PER_LEG; j++) {
            //        if (observations[(areaIndex * observationIndex) + j + OFSET] >= 0) {
            //            rewards -= observations[(areaIndex * observationIndex) + j + OFSET];
            //        }
            //    }
            //}
            //return rewards;
            return 0f;
        }

        int numberOfHits;
        readonly Collider[] hitColliders = new Collider[40];
        //private void SetObservation() {
        //    Transform temp = incommingAreas[0];
        //    numberOfHits = Physics.OverlapBoxNonAlloc(temp.position, temp.localScale / 2, hitColliders, Quaternion.identity, LayerMask.GetMask("Vehicle"));
        //    //for (int i = 0; i < NUM_OF_OBSERVATIONS_PER_VEHICLE; i++) {
        //    int observationIndex = 0;


        //    for (int areaIndex = 0; areaIndex < NUM_OF_AREA; areaIndex++) {
        //        for (int j = 0; j < NUM_OF_VEHICLES_PER_AREA; j++) {
        //            if (j < numberOfHits) {
        //                observations[(areaIndex * observationIndex) + j + OFSET] = Vector3.Distance(selfPosition, hitColliders[j].transform.position);
        //            }
        //            else {
        //                observations[(areaIndex * observationIndex) + j + OFSET] = -1;
        //            }
        //        }

        //    }

        //    observationIndex = 1;
        //    for (int areaIndex = 0; areaIndex < NUM_OF_AREA; areaIndex++) {
        //        for (int j = 0; j < NUM_OF_VEHICLES_PER_AREA; j++) {
        //            if (j < numberOfHits) {
        //                if (hitColliders[j].TryGetComponent(out vdc))
        //                    observations[(areaIndex * observationIndex) + j + OFSET] = vdc.WaitTimeBeforeReachingIntersesction;
        //            }
        //            else {
        //                observations[(areaIndex * observationIndex) + j + OFSET] = -1;
        //            }
        //        }
        //    }
        //}



        public (int, float) GetNextPhase() {
            AddReward(CalculateReward());
            //Debug.Log(GetCumulativeReward());
            EndEpisode();

            print("Decision requested");
            Academy.Instance.EnvironmentStep();
            print("Decision complete");


            ChangeToNextPhaseWithTimeInterpolate(action);
            return (-1, greenLightTime);
        }

        public override void CollectObservations(VectorSensor sensor) {
            //base.CollectObservations(sensor);
            sensor.AddObservation(observations);
        }

        public override void OnActionReceived(ActionBuffers actions) {
            //base.OnActionReceived(actions);
            action = actions.ContinuousActions[0];
            print($"action received: {action}");
        }
    }

}

