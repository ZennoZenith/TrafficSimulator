using UnityEngine;

[CreateAssetMenu(fileName = "RoadTypeScriptableObject", menuName = "ScriptableObjects/RoadTypeScriptableObject", order = 1)]
public class RoadTypeScriptableObject : ScriptableObject {
    public GameObject roadTypePrefab;
    public bool isIntersection;

}
