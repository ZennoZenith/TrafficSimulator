using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static VehicleSpawnerManager;

[RequireComponent(typeof(VehicleSpawnerManager))]
public class GraphGenerator : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    [SerializeField] private VehicleSpawnerManager vehicleSpawnerManager;
    public Graph DirectedGraph { get; private set; } = new();

    private readonly List<RoadSetup> nodes = new();

    private void Awake() {
        vehicleSpawnerManager = GetComponent<VehicleSpawnerManager>();
        GenerateGraph();

    }

    private void Start() {
        //GenerateGraph();
    }
    public void GenerateGraph() {
        // Get all children gameobject with "RoadSetup" component
        nodes.Clear();
        DirectedGraph.Clear();

        foreach (RoadSetup node in transform.GetComponentsInChildren<RoadSetup>()) {
            node.ConvertSplinesToVectors(gameSettings.splineResolution);
            nodes.Add(node);
        }

        //Debug.Log($"Number of nodes: {nodes.Count}");

        // Setup adjecent road connector for each node
        foreach (var node in nodes) {
            node.Clear();
            var froms = node.GetFromConnectors();
            for (int i = 0; i < froms.Length; i++) {
                froms[i].SetAdjecentRoadConnector();
            }
            var tos = node.GetToConnectors();
            for (int i = 0; i < tos.Length; i++) {
                //if (tos[i].connector == null) continue;
                tos[i].SetAdjecentRoadConnector();
            }

            // Add node to directed graph
            DirectedGraph.AddNode(node);
        }

        // Setup edges of graph
        foreach (var node in nodes) {
            foreach (var outgoingConnector in node.GetToConnectors()) {
                if (outgoingConnector.AdjecentRoadConnector == null) continue;

                foreach (var incommingRoadSetup in node.GetAdjecentIncommingRoadSetupForOutgoingConnector(outgoingConnector)) {
                    DirectedGraph.AddEdge(new EdgeData(node, outgoingConnector.AdjecentRoadConnector.ParentRoadSetup, incommingRoadSetup, outgoingConnector, outgoingConnector.AdjecentRoadConnector));
                }
            }
            node.Initialize();
        }

        // Setup reachable nodes from current node
        foreach (SpawnerInfo from in vehicleSpawnerManager.Spawners) {
            foreach (RoadSetup to in vehicleSpawnerManager.Despawners) {
                if (from.roadSetup == to) continue;

                if (DirectedGraph.FindShortestPath(from.roadSetup, to) != null) {
                    from.roadSetup.AddReachableNode(to);
                }
            }
        }

        if (gameSettings.printGraphDetails) {
            DirectedGraph.PrintNodes();
            DirectedGraph.PrintGraph();
        }
    }

    #region Debug Methods
#if UNITY_EDITOR

    private void OnDrawGizmosSelected() {
        if (gameSettings.showGraphNode)
            ShowNodes();

        if (gameSettings.showGraphLine)
            ShowGraphEdges();


    }
    private void ShowGraphEdges() {
        foreach (RoadSetup node in nodes) {
            foreach (var edge in node.Edges) {
                Vector3 p1 = edge.Src.transform.position;
                Vector3 p2 = edge.Dest.transform.position;
                float thickness = 3f;

                Handles.Label((p1 + p2) / 2, $"{edge.Weight}");
                Handles.DrawBezier(p1, p2, p1, p2, Color.blue, null, thickness);
            }
        }
    }


    private void ShowNodes() {
        foreach (RoadSetup node in nodes) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(node.transform.position, gameSettings.nodeSphereRadius);
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

# if UNITY_EDITOR
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