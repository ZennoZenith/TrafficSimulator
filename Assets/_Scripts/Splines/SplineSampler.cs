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

    [SerializeField]
    [Range(0f, 10f)]
    private float width;

    float3 position;
    float3 forwardTangent;
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
            spline.Evaluate(splineIndex, time, out position, out forwardTangent, out upVector);
        }
    }

    private void OnDrawGizmos() {
        SetPositionTangentUpvector();
        //Handles.matrix = transform.localToWorldMatrix;
        //Handles.SphereHandleCap(0, position, Quaternion.identity, 1f, EventType.Repaint);

        float3 right = Vector3.Cross(forwardTangent, upVector).normalized;
        float3 p1 = position + (right * width);
        float3 p2 = position + (-right * width);
        Handles.SphereHandleCap(0, p1, Quaternion.identity, 1f, EventType.Repaint);
        Handles.SphereHandleCap(0, p2, Quaternion.identity, 1f, EventType.Repaint);
    }

}
