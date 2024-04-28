using Simulator.ScriptableObject;
using TMPro;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Simulator.Manager {
    public class GameManager : SingletonPersistent<GameManager> {
        public GameSettingsScriptableObject gameSettings;

        [SerializeField] private TextMeshProUGUI FpsCounterUI;
        [SerializeField] private TextMeshProUGUI GameSpeedUILabel;

        void Start() {
            FpsCounter.Setup(gameSettings.updateInterval);
            Application.targetFrameRate = gameSettings.defaultTargetFrameRate;

            Time.timeScale = gameSettings.timeScale;
            UpdateGameSpeedUI();
        }

        void Update() {
            if (gameSettings.showFpsCount)
                FpsCounterUI.text = Mathf.RoundToInt(FpsCounter.CalculateFpsCount()).ToString();

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
            Time.timeScale = gameSettings.timeScale;
            UpdateGameSpeedUI();
        }

        internal void UpdateGameSpeedUI() {
            GameSpeedUILabel.text = Mathf.RoundToInt(Time.timeScale).ToString();
        }

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