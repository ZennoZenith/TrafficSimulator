using UnityEditor;
using UnityEngine;

public class PathFinding : MonoBehaviour {
    [SerializeField] private Graph directedGraph;
    // --- These variables are only used to display shortest path between two Graph Road Block nodes ---
    public Transform fromNode;
    public Transform toNode;
    // ---

    //private void OnDrawGizmosSelected() {
    //    try {
    //        DisplayShortestPathDebug();
    //        List<Vector3> temp = GetListOfBezierCurvePoints(fromNode, toNode);
    //        foreach (var point in temp) {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(point, 1f);
    //        }

    //    }
    //    catch (System.Exception) {
    //        directedGraph.RegenerateGraph();
    //    }

    //}

    //public List<Node> CalculateShortestPath(Transform fromNodeObj, Transform toNodeObj) {
    //    Node fromNode = fromNodeObj.GetComponent<RoadBlock>().Node;
    //    Node toNode = toNodeObj.GetComponent<RoadBlock>().Node;

    //    return directedGraph.DGraph.FindShortestPath(fromNode, toNode);
    //}

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
