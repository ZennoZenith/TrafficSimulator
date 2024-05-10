using Simulator.TrafficSignal;

namespace Simulator.SignalTiming {
    public interface ISignalTimingAlgorithm {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// Item1: index of next phase, -1 if don't care
        /// Item2: green light time duration for next phase
        /// </returns>
        public (int, float) GetNextPhase(TrafficLightSetup trafficLightSetup);

    }

}

