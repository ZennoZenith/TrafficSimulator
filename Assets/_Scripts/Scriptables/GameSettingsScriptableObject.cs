using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettingsScriptableObject : ScriptableObject {
    public int splineResolution;
    public float pathVectorY;

    [Tooltip("Number of next points to consider for heuristic calculation")]
    public int numberOfHeuristicPoints;

    [Header("Spawner")]
    public int retrySpawnTime;

    [Header("Graph")]
    public bool showGraphNode;
    public bool showGraphLine;
    public bool printGraphDetails;
    public float nodeSphereRadius;

    //[Space(5)]
    [Header("Pathfinding")]
    public bool showDebugPathfindingLines;
    public bool showDebugMessage;

}
