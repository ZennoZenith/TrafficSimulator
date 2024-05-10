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


    [RequireComponent(typeof(RoadSetup))]
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


        #region Unity Methods
        private void Awake() {
            RoadSetup = GetComponent<RoadSetup>();

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

            // This loop will run when simulation is not warmed up yet
            while (Time.time < GameManager.GameSettings.warmupTime) {
                (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = staticSignalAlgorithm.GetNextPhase(this);
                ChangePhaseTo(tempNextPhaseIndex, tempNextPhaseGreenLightTime);
                yield return new WaitForSeconds(greenLightTime);
            }


            while (true) {
                (tempNextPhaseIndex, tempNextPhaseGreenLightTime) = signalTimingAlgorithmType switch {
                    TrafficSignalAlogrithm.Static => staticSignalAlgorithm.GetNextPhase(this),
                    TrafficSignalAlogrithm.Dynamic => dynamicSignalAlgorithm.GetNextPhase(this),
                    TrafficSignalAlogrithm.SignalOptimizationML => mlSignalTimingAlgorithm.GetNextPhase(this),
                    TrafficSignalAlogrithm.PhaseOptimizationML => mlPhaseOrderAlgorithm.GetNextPhase(this),
                    _ => staticSignalAlgorithm.GetNextPhase(this),
                };

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

        public float GetPhaseFromSplineIndex(int splineIndex) {
            foreach (var sp in Phases[CurrentPhaseIndex].splineIndex) {
                if (sp == splineIndex) return Phases[CurrentPhaseIndex].greenLightTime - timePassed;
            }
            return -1;
        }

    }
}