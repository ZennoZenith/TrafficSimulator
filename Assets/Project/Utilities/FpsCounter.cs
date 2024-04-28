using UnityEngine;

namespace Utilities {
    public static class FpsCounter {
        private static float updateInterval = 0.5f; // Interval to update FPS count
        private static float accumulatedFPS = 0f; // Accumulated FPS in the current interval
        private static int framesRendered = 0; // Number of frames rendered in the current interval
        private static float timeLeft; // Time left before updating FPS count
        private static float averageFPS = 0f;

        public static void Setup(float updateInterval) {
            FpsCounter.timeLeft = updateInterval;
            FpsCounter.updateInterval = updateInterval;
        }

        public static void GetSetup() {
        }

        public static float CalculateFpsCount() {
            timeLeft -= Time.deltaTime;
            accumulatedFPS += Time.timeScale / Time.deltaTime;
            framesRendered++;

            // Update FPS count
            if (timeLeft <= 0.0f) {
                averageFPS = accumulatedFPS / framesRendered;
                //Debug.Log("FPS: " + averageFPS);
                // Reset counters for the next interval
                accumulatedFPS = 0f;
                framesRendered = 0;
                timeLeft = updateInterval;
            }
            return averageFPS;
        }
    }

}