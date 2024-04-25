using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class VehicleDriverAI : MonoBehaviour {
    //private (RoadSetup, RoadSetup, RoadSetup) graphNodeBuffer = (null, null, null);

    [SerializeField] private VehicleDataScriptableObject vehicleType;
    [SerializeField] private GameSettingsScriptableObject gameSettings;
    [SerializeField] private VehicleController vehicleController;
    [SerializeField] private Transform frontRay;
    [SerializeField] private Transform sideRay;
    [SerializeField] private List<RoadSetup> shortestPathNodes;
    [field: SerializeField] public GraphGenerator GraphGenerator { get; private set; }

    public bool Initialized { get; private set; } = false;
    public Vector3 SpawnedAt { get; private set; }

    public List<Vector3> PointsToFollow { get; private set; } = new();
    public List<int> PointsToNodesIndex { get; private set; } = new();
    public List<float> HeuristicMaxSpeed { get; private set; } = new();

    int pointsToFollowLength;
    int currentFollowingPointIndex;
    int shortestPathNodesLength;
    int currentNodeIndex;


    [Header("Debug")]
    public bool showDebugLines;
    public RoadSetup fromNode;
    public RoadSetup toNode;

    private int ignoreLayer;
    private void Awake() {
        ignoreLayer = ~LayerMask.GetMask("TrafficLight", "Ignore Raycast", "RoadConnector");
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

        shortestPathNodes = GraphGenerator.DirectedGraph.FindShortestPath(fromNode, toNode);
        //if (shortestPathNodes == null && gameSettings.showDebugMessage) {
        if (shortestPathNodes == null) {
            Debug.Log($"No path found between {fromNode.transform.name} and {toNode.transform.name}.");
            DeInitialize();
            return;
        }
        PointsToFollow.Clear();

        if (shortestPathNodes.Count < 2)
            DeInitialize();

        currentNodeIndex = 0;
        shortestPathNodesLength = shortestPathNodes.Count;
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

        MoveDirectionCorrection();
        brakeState = CollisionCorrection();

        return (inputVector, PointsToFollow[currentFollowingPointIndex], HeuristicMaxSpeed[currentNodeIndex], brakeState);
    }

    float hitDistance;
    Transform hitTransform;
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

        return HandleTrafficLightCollision();

        //hit.transform
    }

    private void MoveDirectionCorrection() {
        inputVector = (PointsToFollow[currentFollowingPointIndex] - transform.position).normalized;
    }

    private BrakeState HandleVehicleCollision(VehicleController hitVehicle) {
        hitDistance = hit.distance;
        Vector3 relativeVector = (hitVehicle.transform.forward * hitVehicle.Speed) - (frontRay.transform.forward * vehicleController.Speed);
        //float dot = Vector3.Dot(hitVehicle.transform.forward * hitVehicle.Speed, frontRay.transform.forward * vehicleController.Speed);
        float relativeVelocity = Vector3.Dot(relativeVector, frontRay.transform.forward);
        //float relativeVelocity = hitVehicle.Speed - dot * vehicleController.Speed;
        //Debug.Log(relativeVelocity);

        if (hitVehicle.Speed < 0.2f && hitDistance < 1f) {
            return BrakeState.HandBrake;
        }

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
    private BrakeState HandleTrafficLightCollision() {
        return BrakeState.NoBrake;
    }

    //public bool TrafficLightDetection(out RaycastHit hit) {
    //    // if (Physics.Raycast(item.rayOrigin.position, item.rayOrigin.forward, out hit, item.rayLength))
    //    if (Physics.Raycast(
    //      frontRay.position,
    //      new Vector3(frontRay.forward.x, 0, frontRay.forward.z),
    //      out hit,
    //      frontRayLength,
    //      LayerMask.GetMask("TrafficLight"))) {
    //        return true;
    //    }
    //    return false;
    //}

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

            //Check if there has been a hit yet
            if (m_HitDetect) {
                //Draw a Ray forward from GameObject toward the hit
                Gizmos.DrawLine(frontRay.position, m_Hit.point);
                //Draw a cube that extends to where the hit exists
                Gizmos.DrawWireSphere(m_Hit.point, 0.3f);
            }

            Gizmos.DrawRay(frontRay.position, frontRay.forward * gameSettings.frontRaySensorLength);
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