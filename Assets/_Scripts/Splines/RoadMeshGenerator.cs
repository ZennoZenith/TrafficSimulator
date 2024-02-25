using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RoadMeshGenerator : MonoBehaviour {
    [SerializeField]
    private RoadMeshType roadMeshType;

    [SerializeField] private SplineContainer[] splines;

    float3 position;
    float3 tangent;
    float3 upVector;



}
