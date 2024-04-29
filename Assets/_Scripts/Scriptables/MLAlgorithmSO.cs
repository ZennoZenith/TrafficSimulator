using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm", fileName = "MLAlgorithm")]
    public class MLAlgorithmSO : UnityEngine.ScriptableObject {

        public ML_Algorithm mL_Algorithm;
        public int REWARD_MULTIPLYER1 = 2000;

    }

    public enum ML_Algorithm {
        None,
        SignalOptimization,
        PhaseOptimization
    }
}