using Simulator.Graph;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Simulator.Road {
    public class RoadConnector : MonoBehaviour, IInitializable {
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
        public bool IsInitialized { get; private set; }
        public Node GraphNode { get; private set; }


        #region Unity Methods
        private void Awake() {
            SetParentRoadSetup();
        }

        //private void OnEnable() {
        //    Initialise();
        //}

        #endregion

        public void Initialize() {
            SetAdjecentRoadConnector();
            SetupGraphNode();
            IsInitialized = true;
        }

        public void DeInitialize() {
            GraphNode = null;
            AdjecentRoadConnector = null;
            IsInitialized = false;
        }


        private RoadSetup SetParentRoadSetup() {
            if (ParentRoadSetup != null) {
                return ParentRoadSetup;
            }
            ParentRoadSetup = transform.parent.parent.GetComponent<RoadSetup>();
            if (ParentRoadSetup == null) {
                Debug.LogError("Unable to set parent road setup reference", transform);
            }
            return ParentRoadSetup;
        }

        private bool SetAdjecentRoadConnector() {

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


        private void SetupGraphNode() {
            if (roadConnectorType == RoadConnectorType.Incomming) return;

            GraphNode = new($"{GraphGenerator.character}{GraphGenerator.number}", transform.position, ParentRoadSetup);

            GraphGenerator.number++;
            if (GraphGenerator.number == 9) {
                GraphGenerator.character++;
                GraphGenerator.number = 0;
            }

            if (AdjecentRoadConnector != null) {
                AdjecentRoadConnector.GraphNode = GraphNode;
            }
        }

    }
}