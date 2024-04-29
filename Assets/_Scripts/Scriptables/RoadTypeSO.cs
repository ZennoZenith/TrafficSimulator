using UnityEngine;

namespace Simulator.ScriptableObject {

    [CreateAssetMenu(fileName = "RoadTypeScriptableObject", menuName = "ScriptableObjects/RoadTypeScriptableObject", order = 1)]
    public class RoadTypeSO : UnityEngine.ScriptableObject {
        public bool isIntersection;

    }
}
