using UnityEngine;

[CreateAssetMenu(fileName = "GenericVehicle", menuName = "ScriptableObjects/VehicleType", order = 1)]
public class VehicleDataScriptableObject : ScriptableObject {

    [Tooltip("in kg")]
    public float mass;

    [Tooltip("in kmph")]
    public float maxSpeed;

}
