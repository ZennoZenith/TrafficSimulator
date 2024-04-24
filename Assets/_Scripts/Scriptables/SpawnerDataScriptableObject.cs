using UnityEngine;


[System.Serializable]
public struct VehicleFrequency {
    public GameObject vehicle;
    public int frequency;
}

[CreateAssetMenu(fileName = "SpawnerData", menuName = "ScriptableObjects/SpawnerData", order = 2)]
public class SpawnerDataScriptableObject : ScriptableObject {
    [Tooltip("Vehicle spawned per minute")]
    public float spawnFrequency;

    [Tooltip("+- change in frequency")]
    public float frequencyVariation;

    public VehicleFrequency[] vehicles;
}
