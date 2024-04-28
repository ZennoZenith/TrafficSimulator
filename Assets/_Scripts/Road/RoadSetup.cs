using Simulator.Graph;
using Simulator.ScriptableObject;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Simulator.Road {

    [RequireComponent(typeof(SplineContainer))]
    public class RoadSetup : MonoBehaviour {

        [System.Serializable]
        public struct RoutesPriorities {
            public int splineIndex;
            public int priority;
        }

        [System.Serializable]
        public struct RoutesMap {
            public RoadConnector from;
            public RoadConnector to;
            public RoutesPriorities[] routes;
        }


        [SerializeField] private GameSettingsScriptableObject gameSettings;
        [field: SerializeField] public RoadTypeScriptableObject RoadTypeSO { get; private set; }
        [Tooltip("in meter per second")]
        [field: SerializeField] public float MaxAllowedSpeed { get; private set; }
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private int overridenSplineResolution = 50;
        [SerializeField] private bool overrideSplineResolution;
        [field: SerializeField] public Transform[] SpawnPoints { get; private set; }
        [SerializeField] private RoadConnector[] incomming;
        [SerializeField] private RoadConnector[] outgoing;
        [SerializeField] private RoutesMap[] routesMap;
        public List<List<Vector3>> RoutesAsVectors { get; private set; } = new();
        public bool Initialized { get; private set; } = false;
        public List<RoadSetup> ReachableNodes { get; private set; } = new();

        public void AddReachableNode(RoadSetup node) {
            ReachableNodes.Add(node);
        }

        public RoadConnector[] GetFromConnectors() {
            return incomming;
        }

        public RoadConnector[] GetToConnectors() {
            return outgoing;
        }

        private void Awake() {
            if (splineContainer == null)
                splineContainer = GetComponent<SplineContainer>();
        }

        public void Initialize() {
            Initialized = true;
        }

        public void DeInitialize() {
            ReachableNodes.Clear();
            Initialized = false;
        }


        public Transform GetSpawnPointNearestTo(Vector3 point) {
            if (SpawnPoints.Length < 1)
                return null;
            float minDistance = Vector3.Distance(point, SpawnPoints[0].position);
            Transform nearestSpawnPoint = SpawnPoints[0];
            for (int i = 1; i < SpawnPoints.Length; i++) {
                float distance = Vector3.Distance(point, SpawnPoints[i].position);
                if (distance < minDistance) {
                    minDistance = distance;
                    nearestSpawnPoint = SpawnPoints[i];
                }
            }
            return nearestSpawnPoint;
        }


        /// <summary>
        /// Use this function when initializing graph
        /// sets vector.y to 0
        /// </summary>
        /// <param name="resolution"></param>
        public void ConvertSplinesToVectors(int resolution) {
            int numberOfSplines = splineContainer.Splines.Count;

            if (overrideSplineResolution)
                resolution = overridenSplineResolution;

            RoutesAsVectors.Clear();
            for (int i = 0; i < numberOfSplines; i++) {
                RoutesAsVectors.Add(new List<Vector3> { SetVectorY(splineContainer.EvaluatePosition(i, 0)) });
                for (int j = 1; j <= resolution; j++) {
                    RoutesAsVectors[i].Add(SetVectorY(splineContainer.EvaluatePosition(i, (float)j / (resolution + 1))));
                }
                RoutesAsVectors[i].Add(SetVectorY(splineContainer.EvaluatePosition(i, 1)));
            }
        }

        private Vector3 SetVectorY(float3 position) {
            return new Vector3(position.x, gameSettings.pathVectorY, position.z);
        }

        public EdgeData GetIncommingConnector(RoadSetup adjecentIncommingNode) {
            foreach (var incommingNode in incomming) {
                if (incommingNode.AdjecentRoadConnector.ParentRoadSetup == adjecentIncommingNode) {
                    foreach (var edge in incommingNode.AdjecentRoadConnector.ParentRoadSetup.Edges) {
                        if (edge.Dest == this)
                            return edge;
                    }
                }
                //if (edge.Dest == adjecentIncommingNode)
                //    return edge;
            }
            return null;
        }

        public EdgeData GetOutgoingConnector(RoadSetup adjecentOutgoingNode) {
            foreach (var edge in edges) {
                if (edge.Dest == adjecentOutgoingNode)
                    return edge;
            }
            return null;
        }

        public List<Vector3> GetRouteFromToNode(RoadSetup fromNode, RoadSetup toNode) {
            if (fromNode == null && toNode == null)
                return null;

            if (fromNode == null && toNode != null) {
                foreach (var route in routesMap) {
                    if (route.to.AdjecentRoadConnector == null)
                        continue;
                    if (route.to.AdjecentRoadConnector.ParentRoadSetup == toNode) {
                        int splineIndex = route.routes[0].splineIndex;
                        return RoutesAsVectors[splineIndex];
                    }
                }
                return null;
            }

            if (fromNode != null && toNode == null) {
                foreach (var route in routesMap) {
                    if (route.from.AdjecentRoadConnector == null)
                        continue;
                    if (route.from.AdjecentRoadConnector.ParentRoadSetup == fromNode) {
                        int splineIndex = route.routes[0].splineIndex;
                        return RoutesAsVectors[splineIndex];
                    }
                }
                return null;
            }

            foreach (var route in routesMap) {
                if (route.from.AdjecentRoadConnector == null)
                    continue;
                if (route.to.AdjecentRoadConnector == null)
                    continue;
                if (route.from.AdjecentRoadConnector.ParentRoadSetup == fromNode
                    && route.to.AdjecentRoadConnector.ParentRoadSetup == toNode) {
                    int splineIndex = route.routes[MaxIndex(route.routes)].splineIndex;
                    //return routesAsVectors[route.routes.Max(t => t.priority)];
                    return RoutesAsVectors[splineIndex];

                }
            }

            return null;
        }

        public int GetSplineIndexFromToNode(RoadSetup fromNode, RoadSetup toNode) {
            if (fromNode == null && toNode == null)
                return -1;

            if (fromNode == null && toNode != null) {
                foreach (var route in routesMap) {
                    if (route.to.AdjecentRoadConnector == null)
                        continue;
                    if (route.to.AdjecentRoadConnector.ParentRoadSetup == toNode) {
                        int splineIndex = route.routes[0].splineIndex;
                        return splineIndex;
                    }
                }
                return -1;
            }

            if (fromNode != null && toNode == null) {
                foreach (var route in routesMap) {
                    if (route.from.AdjecentRoadConnector == null)
                        continue;
                    if (route.from.AdjecentRoadConnector.ParentRoadSetup == fromNode) {
                        int splineIndex = route.routes[0].splineIndex;
                        return splineIndex;
                    }
                }
                return -1;
            }

            foreach (var route in routesMap) {
                if (route.from.AdjecentRoadConnector == null)
                    continue;
                if (route.to.AdjecentRoadConnector == null)
                    continue;
                if (route.from.AdjecentRoadConnector.ParentRoadSetup == fromNode
                    && route.to.AdjecentRoadConnector.ParentRoadSetup == toNode) {
                    int splineIndex = route.routes[MaxIndex(route.routes)].splineIndex;
                    //return routesAsVectors[route.routes.Max(t => t.priority)];
                    return splineIndex;

                }
            }

            return -1;
        }

        private int MaxIndex(RoutesPriorities[] route) {
            int max = 0;
            int maxIndex = 0;
            for (int i = 0; i < route.Length; i++) {

                if (route[i].priority > max) {
                    max = route[i].priority;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }

        public List<RoadSetup> GetAdjecentIncommingRoadSetupForOutgoingConnector(RoadConnector outgoingConnector) {
            List<RoadSetup> roadSetups = new();
            foreach (var route in routesMap) {
                if (route.to == outgoingConnector && route.from.AdjecentRoadConnector != null) {
                    roadSetups.Add(route.from.AdjecentRoadConnector.ParentRoadSetup);
                }
            }
            return roadSetups;
        }


        #region Graph related     
        private readonly List<EdgeData> edges = new();
        public List<EdgeData> Edges {
            get {
                return edges;
            }
        }

        public bool AddEdge(EdgeData edgeData) {
            edges.Add(edgeData);
            return true;
        }

        public bool CheckEdge(RoadSetup dest) {
            if (GetEdge(dest) == null)
                return false;
            return true;
        }

        public EdgeData GetEdge(RoadSetup dest) {
            return edges.Find(ne => ne.Dest == dest);
        }

        public void Clear() {
            edges.Clear();
        }

        #endregion

        //[SerializeField] private Transform 


    }
}