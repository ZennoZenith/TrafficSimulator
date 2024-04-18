using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    public Graph DirectedGraph { get; private set; } = new();


    private readonly List<RoadSetup> nodes = new();
    public void GenerateGraph() {
        // Get all children gameobject with "RoadSetup" component
        nodes.Clear();
        DirectedGraph.Clear();

        foreach (RoadSetup node in transform.GetComponentsInChildren<RoadSetup>()) {
            nodes.Add(node);
        }

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

        foreach (var node in nodes) {
            foreach (var outgoingConnector in node.GetToConnectors()) {
                if (outgoingConnector.AdjecentRoadConnector == null) continue;
                DirectedGraph.AddEdge(new EdgeData(node, outgoingConnector.AdjecentRoadConnector.ParentRoadSetup, outgoingConnector, outgoingConnector.AdjecentRoadConnector));
            }
        }

        //DirectedGraph.PrintNodes();
        DirectedGraph.PrintGraph();
    }
}

//[CustomEditor(typeof(GraphGenerator))]
//public class GraphGeneratorEditor : Editor {

//    public override void OnInspectorGUI() {
//        //base.OnInspectorGUI();

//        DrawDefaultInspector();

//        GraphGenerator graphGenerator = (GraphGenerator)target;

//        if (GUILayout.Button("Regenerate Graph")) {
//            graphGenerator.GenerateGraph();
//        }

//    }
//}