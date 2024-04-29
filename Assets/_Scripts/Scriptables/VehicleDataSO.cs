using UnityEngine;
using UnityEngine.Serialization;

namespace Simulator.ScriptableObject {
    [CreateAssetMenu(fileName = "GenericVehicle", menuName = "ScriptableObjects/VehicleType", order = 1)]
    //[FormerlySerializedAs()]
    public class VehicleDataSO : UnityEngine.ScriptableObject {

        [Tooltip("in kg")]
        public float mass;

        [Tooltip("in meter per sec")]
        public float maxSpeed;

        [Tooltip("in meter per sec per sec")]
        public float maxAcceleration;
        public float brakeAcceleration;

        public float turnSpeed;


        [Tooltip("Trigger distance to get next target point")]
        public float triggerDistance = 0.1f;

        /// <summary>
        /// https://www.ford-trucks.com/forums/1639806-wheel-tractive-force-vs-vehicle-speed-and-shift-points.html
        /// </summary>
        [FormerlySerializedAs("acceleration")]
        public AnimationCurve accelerationCurve;
    }

}