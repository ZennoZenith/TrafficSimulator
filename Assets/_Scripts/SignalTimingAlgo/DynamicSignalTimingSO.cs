using Simulator.SignalTiming;
using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm/DynamicSignalTiming", fileName = "DefaultDynamicSignalTiming", order = 2)]
    internal class DynamicSignalTimingSO : UnityEngine.ScriptableObject, ISignalTimingAlgorithm {
        public (int, float) GetNextPhase(TrafficLightSetup trafficLightSetup) {
            throw new System.NotImplementedException();
        }
    }
}


