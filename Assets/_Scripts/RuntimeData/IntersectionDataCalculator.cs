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

        #region Public Fields
        [field: SerializeField] public int TotalNumberOfVehicles { get; private set; } = 0;
        [field: SerializeField] public int TotalNumberOfVehiclesWaitingInIntersection { get; private set; } = 0;
        #endregion


        private int throughput = 0;
        private readonly Dictionary<VehicleDataCalculator, float> vehiclesWaitingInIntersection = new();
        private int waitTimeAtIntersection;

        private string Name;

        #region Unity Methods
        private void Awake() {
            Name = transform.name;
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
            if (vehiclesWaitingInIntersection.ContainsKey(vehicleDataCalculator))
                vehiclesWaitingInIntersection.Add(vehicleDataCalculator, vehicleDataCalculator.TotalWaitTime);
            vehiclesWaitingInIntersection[vehicleDataCalculator] = vehicleDataCalculator.TotalWaitTime;
            TotalNumberOfVehicles++;
            TotalNumberOfVehiclesWaitingInIntersection++;
        }

        internal void VehicleExited(VehicleDataCalculator vehicleDataCalculator) {
            if (vehiclesWaitingInIntersection.ContainsKey(vehicleDataCalculator)) {
                waitTimeAtIntersection = Mathf.RoundToInt(vehicleDataCalculator.TotalWaitTime - vehiclesWaitingInIntersection[vehicleDataCalculator]);
                vehiclesWaitingInIntersection.Remove(vehicleDataCalculator);
                StoreData.WriteIntesectionWaitTime(Name, vehicleDataCalculator.name, waitTimeAtIntersection);
                TotalNumberOfVehiclesWaitingInIntersection--;
            }
        }
    }
}