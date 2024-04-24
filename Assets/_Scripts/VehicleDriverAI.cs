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
    [SerializeField] private Transform carFrontPosition;
    [SerializeField] private List<RoadSetup> shortestPathNodes;
    [field: SerializeField] public GraphGenerator GraphGenerator { get; private set; }

    public bool Initialized { get; private set; } = false;

    public List<Vector3> PointsToFollow { get; private set; } = new();
    public List<int> PointsToNodesIndex { get; private set; } = new();
    public List<float> HeuristicMaxSpeed { get; private set; } = new();

    int pointsToFollowLength;
    int currentFollowingPointIndex;
    int shortestPathNodesLength;
    public int currentNodeIndex;


    [Header("Debug")]
    public bool showDebugLines;
    public RoadSetup fromNode;
    public RoadSetup toNode;

    public void Initialize(GraphGenerator gg, RoadSetup fromNode, RoadSetup toNode) {
        this.fromNode = fromNode;
        this.toNode = toNode;

        Initialize(gg);
    }
    public void Initialize(GraphGenerator gg) {
        if (gg == null && GraphGenerator == null) {
            return;
        }
        else if (GraphGenerator == null && gg != null) {
            GraphGenerator = gg;
        }

        shortestPathNodes = GraphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);
        if (shortestPathNodes == null && gameSettings.showDebugMessage) {
            Debug.Log($"No path found between {fromNode.transform.name} and {toNode.transform.name}.");
        }
        PointsToFollow.Clear();

        if (shortestPathNodes.Count < 2)
            DeInitialize();

        currentNodeIndex = 0;
        shortestPathNodesLength = shortestPathNodes.Count;
        SetAllPathToFollowVectors();
        CalculateHeuristicSpeeds();

        //SetPathToFollowVectors(shortestPathNodes[currentNodeIndex], null, shortestPathNodes[currentNodeIndex + 1]);

        //if (shortestPathNodesLength > currentNodeIndex + 2)
        //    SetPathToFollowVectors(shortestPathNodes[currentNodeIndex + 1], shortestPathNodes[currentNodeIndex], shortestPathNodes[currentNodeIndex + 2]);



        pointsToFollowLength = PointsToFollow.Count;
        currentFollowingPointIndex = 0;


        //string temp = "";
        //foreach (var item in HeuristicMaxSpeed) {
        //    temp += $"{item}, ";
        //}
        //Debug.Log(temp);

        Initialized = true;
    }

    public void DeInitialize() {
        PointsToFollow.Clear();
        PointsToNodesIndex.Clear();
        HeuristicMaxSpeed.Clear();

        Initialized = false;
    }

    private int SetAllPathToFollowVectors() {

        // For first node
        //var found = shortestPathNodes[0].GetRouteFromConnectors(null, edgeData.FromRoadConnector);
        int numberOfPointsAdded = 0;
        var found = shortestPathNodes[0].GetRouteFromToNode(null, shortestPathNodes[1]);
        if (found != null) {
            PointsToFollow.AddRange(found);
            numberOfPointsAdded += found.Count;
            PointsToNodesIndex.Add(numberOfPointsAdded);
        }

        // For in between nodes
        int len = shortestPathNodes.Count;
        for (int i = 1; i < len - 1; i++) {
            //found = shortestPathNodes[i].GetRouteFromConnectors(previousEdge.ToRoadConnector, edgeData.FromRoadConnector);
            found = shortestPathNodes[i].GetRouteFromToNode(shortestPathNodes[i - 1], shortestPathNodes[i + 1]);

            if (found != null) {
                PointsToFollow.AddRange(found);
                numberOfPointsAdded += found.Count;
                PointsToNodesIndex.Add(numberOfPointsAdded);
            }
        }
        // -------------

        // For last node
        found = shortestPathNodes[len - 1].GetRouteFromToNode(shortestPathNodes[len - 2], null);
        if (found != null) {
            PointsToFollow.AddRange(found);
            numberOfPointsAdded += found.Count;
            PointsToNodesIndex.Add(numberOfPointsAdded);
        }
        // -------------

        return numberOfPointsAdded;

    }


    private void CalculateHeuristicSpeeds() {
        int i;
        for (i = 0; i < shortestPathNodesLength - gameSettings.numberOfHeuristicPoints; i++) {
            float speed = shortestPathNodes[i].MaxAllowedSpeed;
            // Heuristic calculation algorithm
            for (int j = 0; j < gameSettings.numberOfHeuristicPoints; j++) {
                speed += Mathf.Clamp(shortestPathNodes[i + j + 1].MaxAllowedSpeed, 0, shortestPathNodes[i].MaxAllowedSpeed);
            }
            speed /= gameSettings.numberOfHeuristicPoints + 1;
            HeuristicMaxSpeed.Add(speed);
        }

        for (; i < shortestPathNodesLength; i++) {
            float speed = 0;
            for (int j = i; j < shortestPathNodesLength; j++) {
                speed += Mathf.Clamp(shortestPathNodes[j].MaxAllowedSpeed, 0, shortestPathNodes[i].MaxAllowedSpeed);
            }
            speed /= shortestPathNodesLength - i;
            HeuristicMaxSpeed.Add(speed);

        }
    }


    private int GetNodeIndexFromVectorIndex(int pointToFollowIndex) {
        for (int i = 0; i < shortestPathNodesLength; i++) {
            if (pointToFollowIndex < PointsToNodesIndex[i])
                return i;
        }
        return shortestPathNodesLength - 1;
    }

    //private int SetPathToFollowVectors(RoadSetup currentNode, RoadSetup fromNode, RoadSetup toNode) {
    //    var found = currentNode.GetRouteFromToNode(fromNode, toNode);
    //    int numberOfPointsAdded = 0;
    //    if (found != null) {
    //        PointsToFollow.AddRange(found);
    //        numberOfPointsAdded += found.Count;
    //    }
    //    return numberOfPointsAdded;
    //}


    public Vector3[] GetVectorsFromSpline(Spline spline, Transform transformPoint) {
        Vector3[] points = { };
        //Debug.Log(spline.EvaluatePosition(t));
        points.Append(transformPoint.TransformPoint(spline.EvaluatePosition(t)));
        return points;
    }

    float forwardAmount;
    float turnAmount;
    Vector3 inputVector;

    //public (Vector3, bool) GetNextPointToFollow() {
    //    while (currentFollowingPointIndex < pointsToFollowLength) {
    //        if (Vector3.Distance(PointsToFollow[currentFollowingPointIndex], transform.position) < vehicleType.triggerDistance) {
    //            currentFollowingPointIndex++;
    //            currentNodeIndex = GetNodeIndexFromVectorIndex(currentFollowingPointIndex);
    //        }
    //        else { break; }

    //    }

    //    if (currentFollowingPointIndex >= pointsToFollowLength)
    //        return (PointsToFollow[pointsToFollowLength - 1], false);

    //    return (PointsToFollow[currentFollowingPointIndex], false);

    //}


    /// <summary>
    /// <br>Calculates AI Input</br>
    /// </summary>
    /// <returns>
    /// <br>item1: inputVector</br>
    /// <br>item2: targetPosition</br>
    /// <br>item3: targetSpeed</br>
    /// <br>item4: BrakeState</br>
    /// </returns>
    public (Vector3, Vector3, float, BrakeState) CalculateAiInput() {
        if (!Initialized)
            return (Vector3.zero, Vector3.zero, 0f, BrakeState.HandBrake);


        while (currentFollowingPointIndex < pointsToFollowLength) {
            if (Vector3.Distance(PointsToFollow[currentFollowingPointIndex], transform.position) < vehicleType.triggerDistance) {
                currentFollowingPointIndex++;
                currentNodeIndex = GetNodeIndexFromVectorIndex(currentFollowingPointIndex);
            }
            else { break; }

        }

        if (currentFollowingPointIndex >= pointsToFollowLength) {
            DeInitialize();
            return (Vector3.zero, Vector3.zero, 0f, BrakeState.HandBrake);
            //return (Vector3.zero, PointsToFollow[pointsToFollowLength - 1], 0f, BrakeState.HandBrake);
        }
        //return (PointsToFollow[currentFollowingPointIndex], false);

        MoveDirectionCorrection();
        return (inputVector, PointsToFollow[currentFollowingPointIndex], HeuristicMaxSpeed[currentNodeIndex], BrakeState.NoBrake);

        //TurnSpeedCorrection();
        //(inputVector, isBrakePressed) = vehicleDriverAI.GetData();

    }

    private void MoveDirectionCorrection() {
        //forwardAmount = 0f;
        //turnAmount = 0f;

        //Vector3 dirToMovePositionNormalizedLS = carFrontPosition.InverseTransformDirection(PointsToFollow[currentFollowingPointIndex] - carFrontPosition.position).normalized;
        //forwardAmount = dirToMovePositionNormalizedLS.z;
        //if (Mathf.Abs(forwardAmount) < 0.2f)
        //    forwardAmount = -0.5f;

        //turnAmount = Mathf.Sign(forwardAmount) * dirToMovePositionNormalizedLS.x;

        //inputVector.x = turnAmount;
        //inputVector.z = forwardAmount;

        inputVector = (PointsToFollow[currentFollowingPointIndex] - transform.position).normalized;


    }

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


            Gizmos.color = Color.black;
            if (currentFollowingPointIndex < pointsToFollowLength)
                Gizmos.DrawSphere(PointsToFollow[currentFollowingPointIndex], 0.3f);
        }
    }

    public void DisplayShortestPathDebug() {
        List<RoadSetup> shortestPathNodes = GraphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);

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
            vehicleDriverAI.Initialize(null, vehicleDriverAI.fromNode, vehicleDriverAI.toNode);
        }

    }
}

#endif