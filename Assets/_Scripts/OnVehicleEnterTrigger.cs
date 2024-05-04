using Simulator.RuntimeData;
using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator {
    public class OnVehicleEnterTrigger : MonoBehaviour {
        [SerializeField] private IntersectionDataCalculator intersectionDataCalculator;
        [SerializeField] private int legIndex;

        #region Unity Methods
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Vehicle"))
                return;
            intersectionDataCalculator.VehicleEntered(other.transform.GetComponent<VehicleDataCalculator>(), legIndex);
        }
        #endregion
    }
}

