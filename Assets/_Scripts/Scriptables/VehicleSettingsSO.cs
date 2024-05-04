using UnityEngine;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings/VehicleSettings", fileName = "DefaultVehicleSettings", order = 5)]
    public class VehicleSettingsSO : UnityEngine.ScriptableObject {
        [Tooltip("Speed which will be considered as rest")]
        public float considerStopSpeed;

        public float frontRaySensorLength;

        [Tooltip("Number of next points to consider for heuristic speed calculation")]
        public int numberOfHeuristicPoints;
    }
}