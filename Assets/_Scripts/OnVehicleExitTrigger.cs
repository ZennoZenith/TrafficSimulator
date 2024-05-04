using Simulator.RuntimeData;
using Simulator.TrafficSignal;
using UnityEngine;

namespace Simulator {
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class OnVehicleExitTrigger : MonoBehaviour {
        #region Public Fields

        #endregion


        private IntersectionDataCalculator intersectionDataCalculator;
        #region Unity Methods
        private void Awake() {
            intersectionDataCalculator = GetComponent<IntersectionDataCalculator>();
        }
        private void OnTriggerExit(Collider other) {
            if (!other.CompareTag("Vehicle"))
                return;
            intersectionDataCalculator.VehicleExited(other.transform.GetComponent<VehicleDataCalculator>());
        }

        #endregion

        #region Private Methods

        #endregion

    }
}

