using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettingsScriptableObject : ScriptableObject {
    [Tooltip("Time after which writing to file happens in sec")]
    public float bufferTime;
    public int defaultTargetFrameRate;

    public float timeScale;

    public int splineResolution;
    public float pathVectorY;

    [Tooltip("Speed which will be considered as rest")]
    public float considerStopSpeed;

    public float frontRaySensorLength;

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

    [Header("Trafic Signal Timings")]
    public float defautGreenTime;
    public float defautYellowTime;
    public Material greenMaterial;
    public Material redMaterial;
    public float minGreenLightTime;
    public float maxGreenLightTime;
    //public float defautRedTime;


}
