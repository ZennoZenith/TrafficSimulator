using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm/MLPhaseOptimization", fileName = "DefaultMLPhaseOptimization", order = 2)]
    internal class MLPhaseOptimizationSO : UnityEngine.ScriptableObject {
        public (int, float) GetNextPhase(IntersectionDataCalculator intersectionDataCalculator) {
            throw new System.NotImplementedException();
        }
    }
}
