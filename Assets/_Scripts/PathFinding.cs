using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathFinding : MonoBehaviour {
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    [SerializeField] private GraphGenerator directedGraph;
    // --- These variables are only used to display shortest path between two Graph Road Block nodes ---
    public RoadSetup fromNode;
    public RoadSetup toNode;
    public bool showDebugLines;
    // ---

    private void OnDrawGizmosSelected() {
        if (gameSettings.showDebugPathfindingLines && showDebugLines)
            DisplayShortestPathDebug();

        //try {
        //    DisplayShortestPathDebug();
        //    List<Vector3> temp = GetListOfBezierCurvePoints(fromNode, toNode);
        //    foreach (var point in temp) {
        //        Gizmos.color = Color.red;
        //        Gizmos.DrawSphere(point, 1f);
        //    }

        //}
        //catch (System.Exception) {
        //    directedGraph.RegenerateGraph();
        //}

    }

    public List<RoadSetup> CalculateShortestPath() {
        return directedGraph.DirectedGraph.FindShortestPath(fromNode, toNode);
    }

    #region Debug Methods

    public void DisplayShortestPathDebug() {
        List<RoadSetup> shortestPathNodes = CalculateShortestPath();

        if (fromNode == null || toNode == null || shortestPathNodes == null)
            return;

        for (int i = 0; i < shortestPathNodes.Count - 1; i++) {
            Vector3 p1 = shortestPathNodes[i].transform.position;
            Vector3 p2 = shortestPathNodes[i + 1].transform.position;
            //Debug.Log(shortestPathNodes[i].name);

            float thickness = 3f;
            // Handles.Label((p1 + p2) / 2, $"{weights[i]}");
#if UNITY_EDITOR

            Handles.DrawBezier(p1, p2, p1, p2, Color.magenta, null, thickness);
#endif
        }
        //Debug.Log(shortestPathNodes[^1].name);

    }

    #endregion

}

#if UNITY_EDITOR
[CustomEditor(typeof(PathFinding))]
public class PathFindingEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        PathFinding pathFinding = (PathFinding)target;

        if (GUILayout.Button("Display Shortest path")) {
            pathFinding.DisplayShortestPathDebug();
        }

    }
}

#endif
