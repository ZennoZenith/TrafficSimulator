using System.Collections.Generic;
using UnityEngine;

namespace Simulator.Road {
    public class RoadConnector : MonoBehaviour {
        public enum RoadConnectorType {
            Incomming,
            Outgoing
        }

        [field: SerializeField]
        public RoadSetup ParentRoadSetup { get; private set; }

        [SerializeField] private float detectionSphereRadius;
        [field: SerializeField] public RoadConnector AdjecentRoadConnector { get; private set; }

        [field: SerializeField]
        public RoadConnectorType roadConnectorType { get; private set; }

        private void Start() {
            SetParentRoadSetup();
        }

        public RoadSetup SetParentRoadSetup() {
            if (ParentRoadSetup != null) {
                return ParentRoadSetup;
            }
            ParentRoadSetup = transform.parent.parent.GetComponent<RoadSetup>();
            if (ParentRoadSetup == null) {
                Debug.LogError("Unable to set parent road setup reference", transform);
            }
            return ParentRoadSetup;
        }

        public bool SetAdjecentRoadConnector() {

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionSphereRadius, LayerMask.GetMask("RoadConnector"));

            List<RoadConnector> tempRoadConnector = new();

            foreach (var hitCollider in hitColliders) {
                //tempRoadSetup.Add()
                if (hitCollider.GetComponent<RoadConnector>() != null) {
                    tempRoadConnector.Add(hitCollider.GetComponent<RoadConnector>());
                }
            }

            foreach (var roadConnector in tempRoadConnector) {
                var tempRoadSetup = roadConnector.SetParentRoadSetup();
                if (tempRoadSetup == ParentRoadSetup)
                    continue;

                if (roadConnector.roadConnectorType != this.roadConnectorType)
                    AdjecentRoadConnector = roadConnector;
            }


            return true;
        }
    }
}