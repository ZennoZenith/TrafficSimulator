using Simulator.SignalTiming;
using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm/MLPhaseOptimization", fileName = "DefaultMLPhaseOptimization", order = 2)]
    internal class MLPhaseOptimizationSO : UnityEngine.ScriptableObject, ISignalTimingAlgorithm {
        public (int, float) GetNextPhase(TrafficLightSetup trafficLightSetup) {
            throw new System.NotImplementedException();
        }
    }
}
