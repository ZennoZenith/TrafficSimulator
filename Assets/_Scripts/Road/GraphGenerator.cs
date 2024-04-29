using Simulator.Graph;
using Simulator.Manager;
using Simulator.ScriptableObject;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Simulator.Road {

    public class GraphGenerator : MonoBehaviour {
        // Start is called before the first frame update
        [SerializeField] private SplineDataSO splineSettings;
        [SerializeField] private DebugSettingsSO debugSettings;
        [SerializeField] private VehicleSpawnerManager vehicleSpawnerManager;
        public Graph.Graph<Node> DirectedGraph { get; private set; } = new();

        private readonly List<Node> nodes = new();

        #region Unity Methods
        private void Awake() {
        }

        private void Start() {
            GenerateGraph();
        }
        #endregion

        public void GenerateGraph() {
            // Get all children gameobject with "RoadSetup" component
            nodes.Clear();
            DirectedGraph.Clear();

            var allRoadSetups = transform.GetComponentsInChildren<RoadSetup>();

            foreach (RoadSetup roadSetup in allRoadSetups) {
                roadSetup.Initialize();
                //node.ConvertSplinesToVectors();
                nodes.Concat(roadSetup.GraphNodes);
            }

            //Debug.Log($"Number of nodes: {nodes.Count}");

            // Add node to directed graph
            foreach (var node in nodes) {
                DirectedGraph.AddNode(node);
            }

            // Setup edges of graph
            foreach (RoadSetup roadSetup in allRoadSetups) {
                roadSetup.SetupNodeEdgesFromRouteMap();
            }

            // Setup edges of graph
            //foreach (var node in nodes) {
            //    foreach (var outgoingConnector in node.GetToConnectors()) {
            //        if (outgoingConnector.AdjecentRoadConnector == null) continue;

            //        foreach (var incommingRoadSetup in node.GetAdjecentIncommingRoadSetupForOutgoingConnector(outgoingConnector)) {
            //            DirectedGraph.AddEdge(new EdgeData(node, outgoingConnector.AdjecentRoadConnector.ParentRoadSetup, incommingRoadSetup, outgoingConnector, outgoingConnector.AdjecentRoadConnector));
            //        }
            //    }
            //    node.Initialize();
            //}

            //Setup reachable nodes from current node
            foreach (Spawner from in vehicleSpawnerManager.Spawners) {
                foreach (DeSpawner to in vehicleSpawnerManager.Despawners) {
                    if (from.RoadConnector == to) continue;

                    if (DirectedGraph.FindShortestPath(from.RoadConnector.GraphNode, to.RoadConnector.GraphNode) != null) {
                        from.RoadConnector.GraphNode.roadSetup.AddReachableNode(to.RoadConnector.GraphNode);
                    }
                }
            }

            if (debugSettings.printGraphDetails) {
                DirectedGraph.PrintNodes();
                DirectedGraph.PrintGraph();
            }
        }

        #region Debug Methods
#if UNITY_EDITOR

        private void OnDrawGizmosSelected() {
            if (debugSettings.showGraphNode)
                ShowNodes();

            if (debugSettings.showGraphLine)
                ShowGraphEdges();


        }
        private void ShowGraphEdges() {
            foreach (var node in nodes) {
                foreach (var edge in node.Edges) {
                    Vector3 p1 = node.position;
                    Vector3 p2 = ((Node)edge.Dest).position;
                    float thickness = 3f;

                    Handles.Label((p1 + p2) / 2, $"{edge.Weight}");
                    Handles.DrawBezier(p1, p2, p1, p2, Color.blue, null, thickness);
                }
            }
        }


        private void ShowNodes() {
            foreach (var node in nodes) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(node.position, debugSettings.nodeSphereRadius);
                //Handles.Label(node.transform.position + new Vector3(0, 5, 0), node.name);
            }
        }

        //private void ShowAllEdges() {
        //    GameObject[] roadObjects = GameObject.FindGameObjectsWithTag("Road");
        //    foreach (GameObject road in roadObjects) {
        //        RoadBlock rb = road.GetComponent<RoadBlock>();
        //        RoadBlock[] item = rb.GetAdgjecentRoadBlocks();
        //        for (int i = 0; i < item.Length; i++) {
        //            if (item[i] == null)
        //                continue;
        //            Gizmos.color = Color.magenta;
        //            // Gizmos.DrawLine(rb.transform.position, item[i].transform.position);
        //            Vector3 p1 = rb.transform.position;
        //            Vector3 p2 = item[i].transform.position;
        //            float thickness = 3f;
        //            UnityEditor.Handles.DrawBezier(p1, p2, p1, p2, Color.red, null, thickness);
        //        }
        //    }

        //}

#endif
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GraphGenerator))]
    public class GraphGeneratorEditor : Editor {

        public override void OnInspectorGUI() {
            //base.OnInspectorGUI();

            DrawDefaultInspector();

            GraphGenerator graphGenerator = (GraphGenerator)target;

            if (GUILayout.Button("Regenerate Graph")) {
                graphGenerator.GenerateGraph();
            }

        }
    }

#endif
}