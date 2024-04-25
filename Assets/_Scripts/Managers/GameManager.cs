using TMPro;
using UnityEditor;
using UnityEngine;

//public class GameManager : SingletonPersistent<GameManager> {
public class GameManager : MonoBehaviour {
    public GameSettingsScriptableObject gameSettings;
    public float updateInterval = 0.5f; // Interval to update FPS count
    private float accumulatedFPS = 0f; // Accumulated FPS in the current interval
    private int framesRendered = 0; // Number of frames rendered in the current interval
    private float timeLeft; // Time left before updating FPS count

    public bool showFpsCount;
    [SerializeField] private TextMeshProUGUI FpsCounterUI;
    [SerializeField] private TextMeshProUGUI GameSpeedUILabel;

    void Start() {
        Time.timeScale = gameSettings.timeScale;
        timeLeft = updateInterval;
        UpdateGameSpeedUI();
    }

    void Update() {
        if (showFpsCount)
            CalculateFpsCount();
    }

    void CalculateFpsCount() {
        timeLeft -= Time.deltaTime;
        accumulatedFPS += Time.timeScale / Time.deltaTime;
        framesRendered++;

        // Update FPS count
        if (timeLeft <= 0.0f) {
            float averageFPS = accumulatedFPS / framesRendered;
            //Debug.Log("FPS: " + averageFPS);
            FpsCounterUI.text = Mathf.RoundToInt(averageFPS).ToString();
            // Reset counters for the next interval
            accumulatedFPS = 0f;
            framesRendered = 0;
            timeLeft = updateInterval;
        }
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
