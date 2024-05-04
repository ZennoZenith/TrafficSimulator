using Simulator.TrafficSignal;

namespace Simulator.SignalTiming {
    internal class StaticSignalTiming : ISignalTimingAlgorithm {

        private TrafficLightSetup trafficLightSetup;
        public StaticSignalTiming(TrafficLightSetup trafficLightSetup) {
            this.trafficLightSetup = trafficLightSetup;
        }

        public (int, float) GetNextPhase() {
            return (-1, -1);
        }
    }
}

