using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineSampler : MonoBehaviour {
    private SplineContainer[] splines;

    [SerializeField] private int splineIndex;

    //[SerializeField]
    //[Range(0f, 1f)]
    //private float time;


    float3 position;
    float3 forwardTangent;
    float3 upVector;

    private void Start() {
        //if (splines?.Length > 0) {
        //    return;
        //}
        splines = GetComponents<SplineContainer>();

    }

    //private void SetPositionTangentUpvector() {
    //    foreach (var spline in splines) {
    //        spline.Evaluate(splineIndex, time, out position, out forwardTangent, out upVector);
    //    }
    //}

    public void SampleSplineWidth(float time, float width, out Vector3 p1, out Vector3 p2) {
        splines[splineIndex].Evaluate(splineIndex, time, out position, out forwardTangent, out upVector);
        float3 right = Vector3.Cross(forwardTangent, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
    }

}
