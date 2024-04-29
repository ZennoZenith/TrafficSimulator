using Simulator.Graph;
using Simulator.Road;
using Simulator.ScriptableObject;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Simulator.Manager {
    public class GameManager : SingletonPersistent<GameManager> {
        public GameSettingsSO gameSettings;
        public GraphicSettingsSO graphicsSettings;

        [SerializeField] private TextMeshProUGUI FpsCounterUI;
        [SerializeField] private TextMeshProUGUI GameSpeedUILabel;

        void Start() {
            ResetGraphicsSettings();
            Time.timeScale = gameSettings.defalultTimeScale;
            UpdateGameSpeedUI();
        }

        void Update() {
            if (graphicsSettings.showFpsCount)
                FpsCounterUI.text = Mathf.RoundToInt(FpsCounter.CalculateFpsCount()).ToString();

        }

        public void ResetGraphicsSettings() {
            FpsCounter.Setup(graphicsSettings.updateInterval);
            Application.targetFrameRate = graphicsSettings.defaultTargetFrameRate;
        }

        public void IncreaseGameSpeed() {
            if (Time.timeScale >= 10f) {
                return;
            }
            Time.timeScale += 1f;
            UpdateGameSpeedUI();
        }

        public void DecreaseGameSpeed() {
            if (Time.timeScale <= 1f) {
                return;
            }
            Time.timeScale -= 1f;
            UpdateGameSpeedUI();
        }

        public void ResetGameSpeed() {
            Time.timeScale = gameSettings.defalultTimeScale;
            UpdateGameSpeedUI();
        }

        internal void UpdateGameSpeedUI() {
            GameSpeedUILabel.text = Mathf.RoundToInt(Time.timeScale).ToString();
        }

        #region Debug
        [Header("Debug")]
        public RoadConnector fromNode;
        public RoadConnector toNode;
        public GraphGenerator graphGenerator;
        public void DisplayShortestPathDebug() {
            List<Node> shortestPathNodes = graphGenerator.DirectedGraph.FindShortestPath(fromNode.GraphNode, toNode.GraphNode);

            if (fromNode == null || toNode == null || shortestPathNodes == null)
                return;

            for (int i = 0; i < shortestPathNodes.Count - 1; i++) {
                Vector3 p1 = shortestPathNodes[i].position;
                Vector3 p2 = shortestPathNodes[i + 1].position;
                //Debug.Log(shortestPathNodes[i].name);

                // Handles.Label((p1 + p2) / 2, $"{weights[i]}");
#if UNITY_EDITOR
                float thickness = 3f;
                Handles.DrawBezier(p1, p2, p1, p2, Color.magenta, null, thickness);
#endif
            }
        }
        #endregion

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GameManager gameSettings = (GameManager)target;

            if (GUILayout.Button("Increase Speed")) {
                gameSettings.IncreaseGameSpeed();
            }
            if (GUILayout.Button("Decrease Speed")) {
                gameSettings.DecreaseGameSpeed();
            }
            if (GUILayout.Button("Reset Speed")) {
                gameSettings.ResetGameSpeed();
            }

        }
    }
#endif


}