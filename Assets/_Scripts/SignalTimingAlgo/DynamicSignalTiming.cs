using Simulator.TrafficSignal;

namespace Simulator.SignalTiming {
    public class DynamicSignalTiming : ISignalTimingAlgorithm {

        private TrafficLightSetup trafficLightSetup;
        public DynamicSignalTiming(TrafficLightSetup trafficLightSetup) {
            this.trafficLightSetup = trafficLightSetup;
        }

        public (int, float) GetNextPhase() {
            throw new System.NotImplementedException();
        }

        //internal float AdaptiveTrafficLight(int numberOfCarsInQueue) {
        //    if (numberOfCarsInQueue == 0)
        //        return 0f;
        //    if (numberOfCarsInQueue == 1)
        //        return minGreenLightTime;
        //    float returnValue = minGreenLightTime + numberOfCarsInQueue * timeToCrossIntersection;
        //    if (returnValue > maxGreenLightTime)
        //        returnValue = maxGreenLightTime;
        //    return returnValue;
        //}
    }
}

