using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RoadSetup))]
public class TrafficLightSetup : MonoBehaviour {
    [System.Serializable]
    public struct Phase {
        public List<int> splineIndex;
        public float greenLightTime;
    }

    [field: SerializeField] public RoadSetup RoadSetup { get; private set; }
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    [SerializeField] private GameObject LineRendererPrefab;
    [SerializeField] private TextMeshPro TimingUI;
    [field: SerializeField] public Phase[] Phases { get; private set; }


    [field: SerializeField] public int CurrentPhaseIndex { get; private set; }
    [field: SerializeField] public int PreviousPhaseIndex { get; private set; }

    private void Awake() {
        RoadSetup = GetComponent<RoadSetup>();
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
            TimingUI.text = (Phases[CurrentPhaseIndex].greenLightTime - timePassed).ToString();
            timePassed++;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SignalCycle() {
        while (true) {
            RenderPhaseSignalLine();
            yield return new WaitForSeconds(Phases[CurrentPhaseIndex].greenLightTime);
            ChangePhaseTo(GetNextPhase());
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





}
