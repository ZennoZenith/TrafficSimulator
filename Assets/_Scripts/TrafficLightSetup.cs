using Simulator.ML;
using Simulator.Road;
using Simulator.RuntimeData;
using Simulator.ScriptableObject;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Simulator.TrafficSignal {
    [RequireComponent(typeof(RoadSetup))]
    public class TrafficLightSetup : MonoBehaviour {
        [System.Serializable]
        public struct Phase {
            public List<int> splineIndex;
            public float greenLightTime;
            public float minGreenLightTime;
            public float maxGreenLightTime;
        }

        [field: SerializeField] public RoadSetup RoadSetup { get; private set; }
        [SerializeField] private GameSettingsScriptableObject gameSettings;
        [SerializeField] private TraficSignalMLData traficSignalMLData;
        [SerializeField] private TrafficSignalMlAgent trafficSignalMlAgent;
        [SerializeField] private GameObject LineRendererPrefab;
        [SerializeField] private TextMeshPro TimingUI;
        [field: SerializeField] public Phase[] Phases { get; private set; }


        [field: SerializeField] public int CurrentPhaseIndex { get; private set; }
        [field: SerializeField] public int PreviousPhaseIndex { get; private set; }

        float greenLightTime;
        bool mlInitialized = false;
        private int lastNumberOfVehicles = 0;
        private float lastCheckedTime = 0;

        private IntersectionDataCalculator intersectionDataCalculator;
        private void Awake() {
            RoadSetup = GetComponent<RoadSetup>();
            traficSignalMLData = GetComponent<TraficSignalMLData>();
            intersectionDataCalculator = GetComponent<IntersectionDataCalculator>();
            trafficSignalMlAgent = GetComponent<TrafficSignalMlAgent>();
        }


        void Start() {
            CurrentPhaseIndex = 0;
            PreviousPhaseIndex = Phases.Length - 1;

            SetupLineRenderer();

            StartCoroutine(SignalCycle());
            StartCoroutine(Tick());
        }

        private List<LineRenderer> lineRenderers;
        void SetupLineRenderer() {
            lineRenderers = new();
            for (int i = 0; i < RoadSetup.RoutesAsVectors.Count; i++) {
                var t = Instantiate(LineRendererPrefab).transform;
                t.SetParent(transform);

                lineRenderers.Add(t.GetComponent<LineRenderer>());
                lineRenderers[i].material = gameSettings.redMaterial;
                lineRenderers[i].positionCount = RoadSetup.RoutesAsVectors[i].Count;
                for (int j = 0; j < RoadSetup.RoutesAsVectors[i].Count; j++) {
                    lineRenderers[i].SetPosition(j, RoadSetup.RoutesAsVectors[i][j]);
                }

            }

        }

        int timePassed = 0;
        IEnumerator Tick() {
            while (true) {
                TimingUI.text = (greenLightTime - timePassed).ToString();
                timePassed++;
                yield return new WaitForSeconds(1f);
            }
        }

        IEnumerator SignalCycle() {
            while (true) {
                if (gameSettings.usML && Time.time > gameSettings.bufferTime && !mlInitialized) {
                    mlInitialized = true;
                }
                int numberOfVehicles = intersectionDataCalculator.TotalNumberOfVehicles;
                float throughput = (numberOfVehicles - lastNumberOfVehicles) / (Time.time - lastCheckedTime);
                lastNumberOfVehicles = numberOfVehicles;
                lastCheckedTime = Time.time;

                if (!mlInitialized)
                    ChangePhaseTo(GetNextPhase());
                else if (gameSettings.mL_Algorithm == ML_Algorithm.SignalOptimization) {
                    (float[] observations, float reward) = traficSignalMLData.GetObservationsAndRewards(CurrentPhaseIndex, throughput);
                    //ChangePhaseTo(GetNextPhase());
                    ChangeToNextPhaseWithTimeInterpolate(trafficSignalMlAgent.ConsumeAction(reward, observations));
                    //print($"reward: {reward}");
                }
                else if (gameSettings.mL_Algorithm == ML_Algorithm.SignalOptimization) {
                    throw new NotImplementedException();
                }




                RenderPhaseSignalLine();
                yield return new WaitForSeconds(greenLightTime);
            }
        }


        int GetNextPhase() {
            return (CurrentPhaseIndex + 1) % Phases.Length;
        }

        void ChangePhaseTo(int nextPhaseIndex) {
            timePassed = 0;
            if (CurrentPhaseIndex == nextPhaseIndex)
                return;

            PreviousPhaseIndex = CurrentPhaseIndex;
            CurrentPhaseIndex = nextPhaseIndex;
            greenLightTime = Phases[CurrentPhaseIndex].greenLightTime;
        }

        public void ChangeToNextPhaseWithTimeInterpolate(float time) {
            timePassed = 0;
            int index = GetNextPhase();
            greenLightTime = Mathf.FloorToInt(Mathf.Lerp(Phases[index].minGreenLightTime, Phases[index].maxGreenLightTime, (time + 1) / 2));
            PreviousPhaseIndex = CurrentPhaseIndex;
            CurrentPhaseIndex = index;
        }

        void RenderPhaseSignalLine() {
            foreach (var splineIndex in Phases[PreviousPhaseIndex].splineIndex) {
                lineRenderers[splineIndex].material = gameSettings.redMaterial;
            }
            foreach (var splineIndex in Phases[CurrentPhaseIndex].splineIndex) {
                lineRenderers[splineIndex].material = gameSettings.greenMaterial;
            }
        }

        public float GetPhaseFromSplineIndex(int splineIndex) {
            foreach (var sp in Phases[CurrentPhaseIndex].splineIndex) {
                if (sp == splineIndex) return Phases[CurrentPhaseIndex].greenLightTime - timePassed;
            }
            return -1;
        }

        //internal float AdaptiveTrafficLight(int numberOfCarsInQueue) {
        //    if (numberOfCarsInQueue == 0)
        //        return 0f;
        //    if (numberOfCarsInQueue == 1)
        //        return minGreenLightTime;
        //    float returnValue = minGreenLightTime + numberOfCarsInQueue * timeToCrossIntersection;
        //    if (returnValue > maxGreenLightTime)
        //        returnValue = maxGreenLightTime;
        //    return returnValue;
        //}

        //private void OnValidate() {

        //}

    }
}