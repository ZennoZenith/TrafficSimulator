using Simulator.Manager;
using Simulator.Road;
using Simulator.ScriptableObject;
using Simulator.SignalTiming;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Simulator.TrafficSignal {
    public enum TrafficSignalAlogrithm {
        Static,
        Dynamic,
        SignalOptimizationML,
        PhaseOptimizationML,
    }

    [System.Serializable]
    public struct Phase {
        public List<int> splineIndex;
        public float greenLightTime;
        //public float minGreenLightTime;
        //public float maxGreenLightTime;
    }


    [RequireComponent(typeof(RoadSetup), typeof(IntersectionDataCalculator))]
    public class TrafficLightSetup : MonoBehaviour {
        #region Public Fields

        public Action OnTickComplete = () => { };
        public Action OnPhaseChange = () => { };

        public TrafficSignalAlogrithm signalTimingAlgorithmType;



        [field: SerializeField] public RoadSetup RoadSetup { get; private set; }
        [field: SerializeField] public Phase[] Phases { get; private set; }
        [field: SerializeField] public int CurrentPhaseIndex { get; private set; }
        [field: SerializeField] public int PreviousPhaseIndex { get; private set; }

        #endregion

        [SerializeField] private SignalSettingsSO trafficSignalSettings;
        //[SerializeField] private GameSettingsSO gameSettings;
        [SerializeField] private GameObject LineRendererPrefab;
        [SerializeField] private TextMeshPro TimingUI;
        [SerializeField] private StaticSignalTimingSO staticSignalAlgorithm;
        [SerializeField] private DynamicSignalTimingSO dynamicSignalAlgorithm;
        [SerializeField] private MLSignalTimingOptimizationSO mlSignalTimingAlgorithm;
        [SerializeField] private MLPhaseOptimizationSO mlPhaseOrderAlgorithm;





        private float greenLightTime;
        public int LastNumberOfVehicles { get; private set; } = 0;
        public float LastCheckedTime { get; private set; } = 0;
        private TrafficSignalMlAgent mLSignalAgent;
        private IntersectionDataCalculator intersectionDataCalculator;



        #region Unity Methods
        private void Awake() {
            RoadSetup = GetComponent<RoadSetup>();
            intersectionDataCalculator = GetComponent<IntersectionDataCalculator>();
            staticSignalAlgorithm = UnityEngine.ScriptableObject.CreateInstance<StaticSignalTimingSO>();
            dynamicSignalAlgorithm = UnityEngine.ScriptableObject.CreateInstance<DynamicSignalTimingSO>();
            mlSignalTimingAlgorithm = UnityEngine.ScriptableObject.CreateInstance<MLSignalTimingOptimizationSO>();
            mlPhaseOrderAlgorithm = UnityEngine.ScriptableObject.CreateInstance<MLPhaseOptimizationSO>();


            if (!TryGetComponent<TrafficSignalMlAgent>(out var temp)) {
                Debug.LogError("ML Agent component not found");
            }
            else {
                mLSignalAgent = temp;
            }


        }

        private void OnEnable() {
            OnTickComplete += UpdateSignalTimingUI;
            OnPhaseChange += RenderPhaseSignalLine;
        }


        void Start() {
            CurrentPhaseIndex = 0;
            PreviousPhaseIndex = Phases.Length - 1;

            SetupLineRenderer();

            StartCoroutine(SignalCycle());
            StartCoroutine(Tick());
        }

        private void OnDisable() {
            OnTickComplete -= UpdateSignalTimingUI;
            OnPhaseChange -= RenderPhaseSignalLine;
        }

        #endregion


        private List<LineRenderer> lineRenderers;
        private void SetupLineRenderer() {
            lineRenderers = new();
            for (int i = 0; i < RoadSetup.RoutesAsVectors.Count; i++) {
                var t = Instantiate(LineRendererPrefab).transform;
                t.SetParent(transform);

                lineRenderers.Add(t.GetComponent<LineRenderer>());
                lineRenderers[i].material = trafficSignalSettings.redMaterial;
                lineRenderers[i].positionCount = RoadSetup.RoutesAsVectors[i].Count;
                for (int j = 0; j < RoadSetup.RoutesAsVectors[i].Count; j++) {
                    lineRenderers[i].SetPosition(j, RoadSetup.RoutesAsVectors[i][j]);
                }

            }

        }

        private int timePassed = 0;
        IEnumerator Tick() {
            while (true) {
                OnTickComplete.Invoke();
                timePassed++;
                yield return new WaitForSeconds(1f);
            }
        }

        IEnumerator SignalCycle() {

            int tempNextPhaseIndex;
            float tempNextPhaseGreenLightTime;
            //float reward;
            //float[] observations;

            // This loop will run when simulation is not warmed up yet
            while (Time.time < GameManager.GameSettings.warmupTime) {
                //(tempNextPhaseIndex, tempNextPhaseGreenLightTime) = staticSignalAlgorithm.GetNextPhase(intersectionDataCalculator);
                ChangePhaseTo(-1, -1);
                yield return new WaitForSeconds(greenLightTime);
            }


            while (true) {
                switch (signalTimingAlgorithmType) {
                    case TrafficSignalAlogrithm.Static:
                        (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = (-1, -1);
                        break;
                    case TrafficSignalAlogrithm.Dynamic:
                        (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = (-1, -1);
                        break;
                    case TrafficSignalAlogrithm.SignalOptimizationML:
                        mlSignalTimingAlgorithm.CalculateRewards(intersectionDataCalculator, mLSignalAgent.Ml_data);
                        Debug.Log($"Reward after function reward: {mLSignalAgent.Ml_data.rewards}");

                        (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = mLSignalAgent.GenerateAction();
                        break;
                    case TrafficSignalAlogrithm.PhaseOptimizationML:
                        (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = (-1, -1);
                        break;
                    default:
                        (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = (-1, -1);
                        break;
                }

                ChangePhaseTo(tempNextPhaseIndex, tempNextPhaseGreenLightTime);
                yield return new WaitForSeconds(greenLightTime);

            }
        }

        private void UpdateSignalTimingUI() {
            if (greenLightTime - timePassed < 0)
                return;

            TimingUI.text = (greenLightTime - timePassed).ToString();
        }

        private void ChangePhaseTo(int nextPhaseIndex, float nextGreenLightTime = -1) {
            timePassed = 0;
            if (CurrentPhaseIndex == nextPhaseIndex)
                return;

            PreviousPhaseIndex = CurrentPhaseIndex;
            if (nextPhaseIndex < 0)
                CurrentPhaseIndex = (CurrentPhaseIndex + 1) % Phases.Length;
            else
                CurrentPhaseIndex = nextPhaseIndex % Phases.Length;

            if (nextGreenLightTime < 0)
                greenLightTime = Phases[CurrentPhaseIndex].greenLightTime;
            else
                greenLightTime = nextGreenLightTime;

            //print($"CurrentPhaseIndex: {CurrentPhaseIndex}");
            OnPhaseChange.Invoke();
        }

        private void RenderPhaseSignalLine() {
            foreach (var splineIndex in Phases[PreviousPhaseIndex].splineIndex) {
                lineRenderers[splineIndex].material = trafficSignalSettings.redMaterial;
            }
            foreach (var splineIndex in Phases[CurrentPhaseIndex].splineIndex) {
                lineRenderers[splineIndex].material = trafficSignalSettings.greenMaterial;
            }
        }

        //private void ChangeToNextPhaseWithTimeInterpolate(float time) {
        //    //int index = (trafficLightSetup.CurrentPhaseIndex + 1) % phases.Length;
        //    //greenLightTime = Mathf.FloorToInt(Mathf.Lerp(phases[index].minGreenLightTime, phases[index].maxGreenLightTime, (time + 1) / 2));
        //    greenLightTime = Mathf.FloorToInt(Mathf.Lerp(phases[index].greenLightTime - 10, phases[index].greenLightTime + 10, (time + 1) / 2));
        //}


        public float GetPhaseFromSplineIndex(int splineIndex) {
            foreach (var sp in Phases[CurrentPhaseIndex].splineIndex) {
                if (sp == splineIndex) return Phases[CurrentPhaseIndex].greenLightTime - timePassed;
            }
            return -1;
        }

    }
}