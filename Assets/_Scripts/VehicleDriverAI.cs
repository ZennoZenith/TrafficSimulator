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
    [SerializeField] private Transform carFrontPosition;
    [SerializeField] private VehicleSpawnerManager vehicleSpawnerManager;
    [SerializeField] private List<RoadSetup> shortestPathNodes;
    public bool Initialized { get; private set; } = false;

    public List<Vector3> PointsToFollow { get; private set; } = new();
    int pointsToFollowLength;
    int currentFollowingPointIndex;

    [Header("Debug")]
    public bool showDebugLines;
    public RoadSetup fromNode;
    public RoadSetup toNode;

    public void Initialize(RoadSetup fromNode, RoadSetup toNode) {
        this.fromNode = fromNode;
        this.toNode = toNode;
        Initialize();
    }

    public void Initialize() {
        shortestPathNodes = graphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);
        if (shortestPathNodes == null && gameSettings.showDebugMessage) {
            Debug.Log($"No path found between {fromNode.transform.name} and {toNode.transform.name}.");
        }
        PointsToFollow.Clear();

        if (shortestPathNodes.Count < 2)
            DeInitialize();

        SetPathToFollowVectors();
        pointsToFollowLength = PointsToFollow.Count;
        currentFollowingPointIndex = 0;
        Initialized = true;
    }

    private void SetPathToFollowVectors() {

        // For first node
        //var found = shortestPathNodes[0].GetRouteFromConnectors(null, edgeData.FromRoadConnector);
        var found = shortestPathNodes[0].GetRouteFromToNode(null, shortestPathNodes[1]);
        if (found != null)
            PointsToFollow.AddRange(found);

        // For in between nodes
        int len = shortestPathNodes.Count;
        for (int i = 1; i < len - 1; i++) {
            //found = shortestPathNodes[i].GetRouteFromConnectors(previousEdge.ToRoadConnector, edgeData.FromRoadConnector);
            found = shortestPathNodes[i].GetRouteFromToNode(shortestPathNodes[i - 1], shortestPathNodes[i + 1]);

            if (found != null)
                PointsToFollow.AddRange(found);
        }
        // -------------

        // For last node
        found = shortestPathNodes[len - 1].GetRouteFromToNode(shortestPathNodes[len - 2], null);
        if (found != null)
            PointsToFollow.AddRange(found);
        // -------------

    }

    public void DeInitialize() {
        Initialized = false;
    }

    public Vector3[] GetVectorsFromSpline(Spline spline, Transform transformPoint) {
        Vector3[] points = { };
        //Debug.Log(spline.EvaluatePosition(t));
        points.Append(transformPoint.TransformPoint(spline.EvaluatePosition(t)));
        return points;
    }

    float forwardAmount;
    float turnAmount;
    Vector3 inputVector;

    public (Vector3, bool) CalculateAiInput() {
        while (currentFollowingPointIndex < pointsToFollowLength) {
            if (Vector3.Distance(PointsToFollow[currentFollowingPointIndex], transform.position) < vehicleType.triggerDistance) {
                currentFollowingPointIndex++;
            }
            else { break; }

        }

        if (currentFollowingPointIndex >= pointsToFollowLength)
            return (PointsToFollow[pointsToFollowLength - 1], true);

        return (PointsToFollow[currentFollowingPointIndex], false);

        //MoveDirectionCorrection();
        //TurnSpeedCorrection();



        //(inputVector, isBrakePressed) = vehicleDriverAI.GetData();

    }

    //private void MoveDirectionCorrection() {
    //    forwardAmount = 0f;
    //    turnAmount = 0f;

    //    Vector3 dirToMovePositionNormalizedLS = carFrontPosition.InverseTransformDirection(PointsToFollow[currentFollowingPointIndex] - carFrontPosition.position).normalized;
    //    forwardAmount = dirToMovePositionNormalizedLS.z;
    //    if (Mathf.Abs(forwardAmount) < 0.2f)
    //        forwardAmount = -0.5f;

    //    turnAmount = Mathf.Sign(forwardAmount) * dirToMovePositionNormalizedLS.x;

    //    inputVector.x = turnAmount;
    //    inputVector.z = forwardAmount;
    //}

    //void TurnSpeedCorrection() {
    //    if (turnTargetSquaredDistance < GameSettings.APPLY_BRAKE_DISTANCE_SQUARE) {
    //        float sqRt = Mathf.Sqrt(turnTargetSquaredDistance);
    //        if (sqRt < 6f)
    //            targetSpeed = GameSettings.TRUN_SPEED;
    //        else
    //            targetSpeed = Util.Interpolate(0, GameSettings.APPLY_BRAKE_DISTANCE, GameSettings.TRUN_SPEED, carPhysicsData.MaxSpeed, sqRt);
    //    }
    //    else {
    //        targetSpeed = -1f;
    //    }
    //}


    #region Debug Methods

    [Range(0f, 2f)]
    public float t;
    private void OnDrawGizmosSelected() {
        if (gameSettings.showDebugPathfindingLines && showDebugLines) {
            DisplayShortestPathDebug();

            Gizmos.color = Color.cyan;
            foreach (var point in PointsToFollow) {
                Gizmos.DrawSphere(point, 0.1f);
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