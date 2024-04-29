using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/SplineData", fileName = "SplineData")]
    public class SplineDataSO : UnityEngine.ScriptableObject {
        public int splineResolution;
        public float pathVectorY;
    }
}