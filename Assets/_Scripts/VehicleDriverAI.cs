using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleDriverAI : MonoBehaviour {
    //private (RoadSetup, RoadSetup, RoadSetup) graphNodeBuffer = (null, null, null);

    [SerializeField] private VehicleDataScriptableObject vehicleType;
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    [SerializeField] private GraphGenerator graphGenerator;
    [SerializeField] private List<RoadSetup> shortestPathNodes;
    [SerializeField] private readonly List<Vector3> pointsToFollow = new();


    [Header("Debug")]
    public bool showDebugLines;
    public RoadSetup fromNode;
    public RoadSetup toNode;

    public void Initialize(RoadSetup fromNode, RoadSetup toNode) {
        shortestPathNodes = graphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);

        if (shortestPathNodes.Count < 2)
            DeInitialize();

        //// Setup graph node buffer
        //graphNodeBuffer.Item1 = null;
        //graphNodeBuffer.Item1 = shortestPathNodes[0];
        //graphNodeBuffer.Item1 = shortestPathNodes[1];

        SetPathToFollowVectors();

    }

    private void SetPathToFollowVectors() {
        pointsToFollow.Clear();

        // For first node
        EdgeData temp = shortestPathNodes[0].GetOutgoingConnector(shortestPathNodes[1]);

        Spline spline = shortestPathNodes[0].GetRouteFromConnectors(null, temp.FromRoadConnector);
        pointsToFollow.Add(shortestPathNodes[0].transform.TransformPoint(spline.EvaluatePosition(t)));
        //Debug.Log(pointsToFollow.Count);
        // -------------

        // For in between nodes

        // -------------

        // For last node
        // -------------

    }

    public void DeInitialize() {
    }


    #region Debug Methods

    [Range(0f, 2f)]
    public float t;
    private void OnDrawGizmosSelected() {
        if (gameSettings.showDebugPathfindingLines && showDebugLines) {
            DisplayShortestPathDebug();

            Gizmos.color = Color.cyan;
            foreach (var point in pointsToFollow) {
                Gizmos.DrawSphere(point, 0.4f);
            }
        }
    }

    public void DisplayShortestPathDebug() {
        List<RoadSetup> shortestPathNodes = graphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);

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

        // -----



    }

    #endregion

}


# if UNITY_EDITOR
[CustomEditor(typeof(VehicleDriverAI))]
public class VehicleDriverAIEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        VehicleDriverAI vehicleDriverAI = (VehicleDriverAI)target;

        if (GUILayout.Button("Initialize")) {
            vehicleDriverAI.Initialize(vehicleDriverAI.fromNode, vehicleDriverAI.toNode);
        }

    }
}

#endif