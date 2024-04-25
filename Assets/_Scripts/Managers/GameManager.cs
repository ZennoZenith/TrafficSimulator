using TMPro;
using UnityEngine;

public class GameManager : SingletonPersistent<GameManager> {
    public float updateInterval = 0.5f; // Interval to update FPS count
    private float accumulatedFPS = 0f; // Accumulated FPS in the current interval
    private int framesRendered = 0; // Number of frames rendered in the current interval
    private float timeLeft; // Time left before updating FPS count

    public bool showFpsCount;
    [SerializeField] private TextMeshProUGUI FpsCounterUI;

    void Start() {
        timeLeft = updateInterval;
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
}
