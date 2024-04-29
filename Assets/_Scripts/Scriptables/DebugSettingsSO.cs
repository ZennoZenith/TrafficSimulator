using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/DebugSettings", fileName = "DefaultDebugSettings", order = 3)]
    public class DebugSettingsSO : UnityEngine.ScriptableObject {
        [Header("Graph")]
        public bool showGraphNode;
        public bool showGraphLine;
        public bool printGraphDetails;
        public float nodeSphereRadius;

        [Header("Pathfinding")]
        public bool showDebugPathfindingLines;
        public bool showDebugMessage;
    }
}