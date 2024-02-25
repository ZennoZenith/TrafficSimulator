using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineSampler : MonoBehaviour {
    [SerializeField] private SplineContainer[] splines;

    [SerializeField] private int splineIndex;

    [SerializeField]
    [Range(0f, 1f)]
    private float time;

    float3 position;
    float3 tangent;
    float3 upVector;

    private void Start() {
        if (splines.Length > 0) {
            return;
        }
        splines = GetComponents<SplineContainer>();

    }

    private void Update() {
        //SetPositionTangentUpvector();
    }

    private void SetPositionTangentUpvector() {
        foreach (var spline in splines) {
            spline.Evaluate(splineIndex, time, out position, out tangent, out upVector);
        }
    }

    private void OnDrawGizmos() {
        SetPositionTangentUpvector();
        //Handles.matrix = transform.localToWorldMatrix;
        Handles.SphereHandleCap(0, position, Quaternion.identity, 1f, EventType.Repaint);
    }

}
