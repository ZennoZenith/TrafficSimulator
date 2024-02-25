using UnityEngine;

[CreateAssetMenu(fileName = "RoadMeshType", menuName = "ScriptableObjects/RoadMeshType", order = 1)]
public class RoadMeshType : ScriptableObject {

    public int resoulution = 10;
    public int numberOfLanes = 1;
    public bool isTwoWay = false;
}
