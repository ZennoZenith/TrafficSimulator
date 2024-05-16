using Simulator.RuntimeData;
using Simulator.ScriptableObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Simulator.TrafficSignal {
    [RequireComponent(typeof(TrafficLightSetup))]
    public class IntersectionDataCalculator : MonoBehaviour {
        [SerializeField] private DataGenerationSettingsSO dataGenerationSetting;
        [SerializeField] private int numberOfLegs;
        //[SerializeField] private int[] vehiclesInLeg;

        #region Public Fields
        [field: SerializeField] public int TotalNumberOfVehicles { get; private set; } = 0;
        [field: SerializeField] public int TotalNumberOfVehiclesWaitingInIntersection { get; private set; } = 0;
        #endregion


        private int throughput = 0;
        // Dictionary value: (legIndex, wait time at interection)
        //private readonly Dictionary<VehicleDataCalculator, (int, float)> vehiclesWaitingInIntersection = new();

        // Dictionary value: (wait time at interection)
        public readonly List<Dictionary<VehicleDataCalculator, float>> vehiclesWaitingAtLeg = new();
        private int waitTimeAtIntersection;

        private string Name;

        #region Unity Methods
        private void Awake() {
            Name = transform.name;
            //vehiclesInLeg = new int[numberOfLegs];
            for (int i = 0; i < numberOfLegs; i++) {
                vehiclesWaitingAtLeg.Add(new Dictionary<VehicleDataCalculator, float>());
            }
        }
        private void Start() {
            StartCoroutine(Tick());
        }
        #endregion

        private IEnumerator Tick() {
            int lastVehicleCount = 0;
            while (true) {
                yield return new WaitForSeconds(dataGenerationSetting.writeIntersectionThroughputPerNSec);
                throughput = (TotalNumberOfVehicles - lastVehicleCount) / dataGenerationSetting.writeIntersectionThroughputPerNSec;
                StoreData.WriteIntesectionThroughput(Name, throughput);
                lastVehicleCount = TotalNumberOfVehicles;
            }
        }

        internal void VehicleEntered(VehicleDataCalculator vehicleDataCalculator, int legIndex) {
            if (vehiclesWaitingAtLeg[legIndex].ContainsKey(vehicleDataCalculator)) {
                vehiclesWaitingAtLeg[legIndex].Add(vehicleDataCalculator, vehicleDataCalculator.TotalWaitTime);

            }
            //vehiclesWaitingAtLeg[vehicleDataCalculator] = (legIndex, vehicleDataCalculator.TotalWaitTime);
            vehiclesWaitingAtLeg[legIndex][vehicleDataCalculator] = vehicleDataCalculator.TotalWaitTime;
            TotalNumberOfVehicles++;
            //vehiclesInLeg[legIndex]++;
            TotalNumberOfVehiclesWaitingInIntersection++;
        }

        internal void VehicleExited(VehicleDataCalculator vehicleDataCalculator) {
            for (int i = 0; i < numberOfLegs; i++) {
                if (vehiclesWaitingAtLeg[i].ContainsKey(vehicleDataCalculator)) {
                    float t = vehiclesWaitingAtLeg[i][vehicleDataCalculator];
                    vehiclesWaitingAtLeg[i].Remove(vehicleDataCalculator);
                    waitTimeAtIntersection = Mathf.RoundToInt(vehicleDataCalculator.TotalWaitTime - t);
                    StoreData.WriteIntesectionWaitTime(Name, vehicleDataCalculator.name, waitTimeAtIntersection);
                    TotalNumberOfVehiclesWaitingInIntersection--;
                    break;
                }

            }
        }
    }
}