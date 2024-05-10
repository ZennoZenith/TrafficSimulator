using Simulator.SignalTiming;
using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/MLAlgorithm/StaticSignalTiming", fileName = "DefaultStaticSignalTiming", order = 2)]
    internal class StaticSignalTimingSO : UnityEngine.ScriptableObject, ISignalTimingAlgorithm {
        public (int, float) GetNextPhase(TrafficLightSetup trafficLightSetup) {
            return (-1, -1);
        }
    }
}

