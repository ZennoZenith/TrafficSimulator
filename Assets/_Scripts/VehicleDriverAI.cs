using System;
using System.Collections.Generic;
using System.Linq;
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

        /* 
         * TODO: Path finding now working for scenerion when
         * there is an incomming node and an outgoing node
         * but there in no spline route that specific incomming 
         * and outgoing node
        */
        shortestPathNodes = graphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);
        pointsToFollow.Clear();

        if (shortestPathNodes.Count < 2)
            DeInitialize();

        //// Setup graph node buffer
        //graphNodeBuffer.Item1 = null;
        //graphNodeBuffer.Item1 = shortestPathNodes[0];
        //graphNodeBuffer.Item1 = shortestPathNodes[1];

        SetPathToFollowVectors();

    }

    private void SetPathToFollowVectors() {

        // For first node
        EdgeData edgeData = shortestPathNodes[0].GetOutgoingConnector(shortestPathNodes[1]);
        EdgeData previousEdge;

        var found = shortestPathNodes[0].GetRouteFromConnectors(null, edgeData.FromRoadConnector);
        if (found != null)
            pointsToFollow.AddRange(found);


        int len = shortestPathNodes.Count;
        previousEdge = edgeData;
        for (int i = 1; i < len - 1; i++) {
            edgeData = shortestPathNodes[i].GetOutgoingConnector(shortestPathNodes[i + 1]);
            found = shortestPathNodes[i].GetRouteFromConnectors(previousEdge.ToRoadConnector, edgeData.FromRoadConnector);
            if (found != null)
                pointsToFollow.AddRange(found);
            previousEdge = edgeData;
        }

        // For in between nodes

        // -------------

        // For last node
        // -------------

    }

    public void DeInitialize() {
    }

    public Vector3[] GetVectorsFromSpline(Spline spline, Transform transformPoint) {
        Vector3[] points = { };
        //Debug.Log(spline.EvaluatePosition(t));
        points.Append(transformPoint.TransformPoint(spline.EvaluatePosition(t)));
        return points;
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