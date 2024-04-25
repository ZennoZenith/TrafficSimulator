using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class VehicleDriverAI : MonoBehaviour {
    //private (RoadSetup, RoadSetup, RoadSetup) graphNodeBuffer = (null, null, null);

    [field: SerializeField] public VehicleDataScriptableObject VehicleType { get; private set; }
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    [SerializeField] private VehicleController vehicleController;
    [SerializeField] private Transform frontRay;
    [SerializeField] private Transform sideRay;
    [field: SerializeField] public List<RoadSetup> ShortestPathNodes { get; private set; }
    [field: SerializeField] public GraphGenerator GraphGenerator { get; private set; }

    public bool Initialized { get; private set; } = false;
    public Vector3 SpawnedAt { get; private set; }

    public List<Vector3> PointsToFollow { get; private set; } = new();
    public List<int> PointsToNodesIndex { get; private set; } = new();
    public List<float> HeuristicMaxSpeed { get; private set; } = new();

    int pointsToFollowLength;
    int currentFollowingPointIndex;
    int shortestPathNodesLength;
    public int CurrentNodeIndex { get; private set; }


    [Header("Debug")]
    public bool showDebugLines;
    public RoadSetup fromNode;
    public RoadSetup toNode;

    private int ignoreLayer;
    private void Awake() {
        ignoreLayer = ~LayerMask.GetMask("Ignore Raycast", "RoadConnector");
        vehicleController = GetComponent<VehicleController>();
    }

    //- TODO: remove
    private void Start() {
        Initialize(null, fromNode, toNode);
    }
    //-

    public void Initialize(GraphGenerator gg, RoadSetup fromNode, RoadSetup toNode) {
        this.fromNode = fromNode;
        this.toNode = toNode;

        Initialize(gg);
    }
    public void Initialize(GraphGenerator gg) {
        if (gg == null && GraphGenerator == null) {
            DeInitialize();
            return;
        }
        else if (GraphGenerator == null && gg != null) {
            GraphGenerator = gg;
        }

        ShortestPathNodes = GraphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);
        //if (shortestPathNodes == null && gameSettings.showDebugMessage) {
        if (ShortestPathNodes == null) {
            Debug.Log($"No path found between {fromNode.transform.name} and {toNode.transform.name}.");
            DeInitialize();
            return;
        }
        PointsToFollow.Clear();

        if (ShortestPathNodes.Count < 2)
            DeInitialize();

        CurrentNodeIndex = 0;
        shortestPathNodesLength = ShortestPathNodes.Count;
        SetAllPathToFollowVectors();
        CalculateHeuristicSpeeds();

        pointsToFollowLength = PointsToFollow.Count;
        currentFollowingPointIndex = 0;

        // Set spawn location
        Transform temp = fromNode.GetSpawnPointNearestTo(PointsToFollow[0]);
        if (temp != null) {
            transform.position = temp.position;
            transform.forward = temp.forward;
        }
        else {
            transform.position = PointsToFollow[0];
            transform.forward = PointsToFollow[1] - PointsToFollow[0];
        }
        SpawnedAt = transform.position;


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
        vehicleController.DeInitialize();
    }

    private int SetAllPathToFollowVectors() {

        // For first node
        //var found = shortestPathNodes[0].GetRouteFromConnectors(null, edgeData.FromRoadConnector);
        int numberOfPointsAdded = 0;
        var found = ShortestPathNodes[0].GetRouteFromToNode(null, ShortestPathNodes[1]);
        if (found != null) {
            PointsToFollow.AddRange(found);
            numberOfPointsAdded += found.Count;
            PointsToNodesIndex.Add(numberOfPointsAdded);
        }

        // For in between nodes
        int len = ShortestPathNodes.Count;
        for (int i = 1; i < len - 1; i++) {
            //found = shortestPathNodes[i].GetRouteFromConnectors(previousEdge.ToRoadConnector, edgeData.FromRoadConnector);
            found = ShortestPathNodes[i].GetRouteFromToNode(ShortestPathNodes[i - 1], ShortestPathNodes[i + 1]);

            if (found != null) {
                PointsToFollow.AddRange(found);
                numberOfPointsAdded += found.Count;
                PointsToNodesIndex.Add(numberOfPointsAdded);
            }
        }
        // -------------

        // For last node
        found = ShortestPathNodes[len - 1].GetRouteFromToNode(ShortestPathNodes[len - 2], null);
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
            float speed = ShortestPathNodes[i].MaxAllowedSpeed;
            // Heuristic calculation algorithm
            for (int j = 0; j < gameSettings.numberOfHeuristicPoints; j++) {
                speed += Mathf.Clamp(ShortestPathNodes[i + j + 1].MaxAllowedSpeed, 0, ShortestPathNodes[i].MaxAllowedSpeed);
            }
            speed /= gameSettings.numberOfHeuristicPoints + 1;
            HeuristicMaxSpeed.Add(speed);
        }

        for (; i < shortestPathNodesLength; i++) {
            float speed = 0;
            for (int j = i; j < shortestPathNodesLength; j++) {
                speed += Mathf.Clamp(ShortestPathNodes[j].MaxAllowedSpeed, 0, ShortestPathNodes[i].MaxAllowedSpeed);
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


    Vector3 inputVector;
    BrakeState brakeState;
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
            if (Vector3.Distance(PointsToFollow[currentFollowingPointIndex], transform.position) < VehicleType.triggerDistance) {
                currentFollowingPointIndex++;
                CurrentNodeIndex = GetNodeIndexFromVectorIndex(currentFollowingPointIndex);
            }
            else { break; }

        }

        if (currentFollowingPointIndex >= pointsToFollowLength) {
            DeInitialize();
            return (Vector3.zero, Vector3.zero, 0f, BrakeState.HandBrake);
            //return (Vector3.zero, PointsToFollow[pointsToFollowLength - 1], 0f, BrakeState.HandBrake);
        }

        MoveDirectionCorrection();
        brakeState = CollisionCorrection();

        return (inputVector, PointsToFollow[currentFollowingPointIndex], HeuristicMaxSpeed[CurrentNodeIndex], brakeState);
    }

    float hitDistance;
    RaycastHit hit;
    //float previousRelativeVelocity;
    //float previousHitTime;
    private BrakeState CollisionCorrection() {
        if (!FrontCollisionDetection(out hit)) {
            //previousRelativeVelocity = 0;
            //previousHitTime = Time.time;
            return BrakeState.NoBrake;
        }

        Transform hitTransform = hit.transform;

        if (hitTransform.TryGetComponent<VehicleController>(out var hitVehicle)) {
            return HandleVehicleCollision(hitVehicle);
        }
        if (hitTransform.TryGetComponent<TrafficLightSetup>(out var hitTrafficSignal)) {
            return HandleTrafficLightCollision(hitTrafficSignal);
        }

        return BrakeState.NoBrake;
    }

    private void MoveDirectionCorrection() {
        inputVector = (PointsToFollow[currentFollowingPointIndex] - transform.position).normalized;
    }

    private BrakeState HandleVehicleCollision(VehicleController hitVehicle) {
        // Set to -1 means that most recent collision occured with vehicle
        routeSplineIndex = -1;


        hitDistance = hit.distance;
        float dot = Vector3.Dot(hitVehicle.transform.forward, frontRay.transform.forward);

        // if angle btw this and hit vehicle is grater than 90+ then ignore collision
        if (dot < -0.2f) {
            return BrakeState.NoBrake;
        }


        Vector3 relativeVector = (hitVehicle.transform.forward * hitVehicle.Speed) - (frontRay.transform.forward * vehicleController.Speed);
        float relativeVelocity = Vector3.Dot(relativeVector, frontRay.transform.forward);
        //float relativeVelocity = hitVehicle.Speed - dot * vehicleController.Speed;
        //Debug.Log(relativeVelocity);

        if (hitDistance < 0.1f) {
            return BrakeState.EmergencyBrake;
        }

        if (hitVehicle.Speed < 0.2f && hitDistance < 0.2f) {
            return BrakeState.HandBrake;
        }

        // if following vehicle is > 8 meters away
        if (hitDistance < 1f && relativeVelocity < 1f) {
            return BrakeState.HandBrake;
        }
        else if (hitDistance < 15f && relativeVelocity < -3f) {
            return BrakeState.Brake;
        }
        else if (hitDistance < 20f && relativeVelocity < -5f) {
            return BrakeState.Brake;
        }

        return BrakeState.NoBrake;
    }

    int routeSplineIndex = -1;
    private BrakeState HandleTrafficLightCollision(TrafficLightSetup hitTrafficSignal) {
        int intersectionNodeIndex;
        if (routeSplineIndex == -1) {

            if (ShortestPathNodes[CurrentNodeIndex].transform == hitTrafficSignal.transform) {
                intersectionNodeIndex = CurrentNodeIndex;
            }
            else if (ShortestPathNodes[CurrentNodeIndex + 1].transform == hitTrafficSignal.transform) {
                intersectionNodeIndex = CurrentNodeIndex + 1;
            }
            else {
                return BrakeState.NoBrake;
            }

            routeSplineIndex = hitTrafficSignal.RoadSetup.GetSplineIndexFromToNode(ShortestPathNodes[intersectionNodeIndex - 1], ShortestPathNodes[intersectionNodeIndex + 1]);

        }

        /// hitTrafficSignal.GetPhaseFromSplineIndex(routeSplineIndex) returns number of seconds remaining for changing signal
        if (routeSplineIndex == -1 || hitTrafficSignal.GetPhaseFromSplineIndex(routeSplineIndex) > 1) {
            return BrakeState.NoBrake;
        }

        hitDistance = hit.distance;
        float relativeVelocity = -vehicleController.Speed;
        // if following vehicle is > 8 meters away
        if (hitDistance < 0.5f && relativeVelocity < 1f) {
            return BrakeState.HandBrake;
        }
        else if (hitDistance < 15f && relativeVelocity < -3f) {
            return BrakeState.Brake;
        }
        else if (hitDistance < 20f && relativeVelocity < -5f) {
            return BrakeState.Brake;
        }

        return BrakeState.NoBrake;
    }


    private bool FrontCollisionDetection(out RaycastHit hit) {

        return Physics.BoxCast(
          frontRay.position,
          frontRay.localScale / 2,
          new Vector3(frontRay.forward.x, 0, frontRay.forward.z),
          out hit,
          frontRay.rotation,
          gameSettings.frontRaySensorLength,
          LayerMask.GetMask("TrafficLight", "Vehicle"));

    }

    internal float DistanceToTravel() {
        float distance = 0f;
        for (int i = 0; i < shortestPathNodesLength - 2; i++) {
            distance += Vector3.Distance(ShortestPathNodes[i].transform.position, ShortestPathNodes[i + 1].transform.position);
        }
        return distance;


    }




    #region Debug Methods

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

            // ----------------------------------------------------------------
            Gizmos.color = Color.red;
            bool m_HitDetect = Physics.BoxCast(frontRay.position, frontRay.localScale / 2, new Vector3(frontRay.forward.x, 0, frontRay.forward.z), out RaycastHit m_Hit, frontRay.rotation, gameSettings.frontRaySensorLength, ignoreLayer);
            //bool m_HitDetect = Physics.BoxCast(frontRay.position, frontRay.localScale / 2, new Vector3(frontRay.forward.x, 0, frontRay.forward.z), out RaycastHit m_Hit, frontRay.rotation, 30, ignoreLayer);

            //Check if there has been a hit yet
            if (m_HitDetect) {
                //Draw a Ray forward from GameObject toward the hit
                Gizmos.DrawLine(frontRay.position, m_Hit.point);
                //Draw a cube that extends to where the hit exists
                Gizmos.DrawWireSphere(m_Hit.point, 0.3f);
            }

            Gizmos.DrawRay(frontRay.position, frontRay.forward * gameSettings.frontRaySensorLength);
            //Gizmos.DrawRay(frontRay.position, frontRay.forward * 30);
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

            // Handles.Label((p1 + p2) / 2, $"{weights[i]}");
#if UNITY_EDITOR
            float thickness = 3f;
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