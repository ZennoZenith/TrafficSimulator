using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(SplineSampler))]
public class RoadMeshGenerator : MonoBehaviour {
    [SerializeField] private SplineSampler splineSampler;

    [SerializeField]
    private RoadMeshType roadMeshType;

    [SerializeField]
    [UnityEngine.Range(1, 200)]
    private float resolution;

    [SerializeField]
    [UnityEngine.Range(0f, 10f)]
    private float width;

    List<Vector3> vertsP1;
    List<Vector3> vertsP2;

    private void Update() {
        SetVerts();
        BuildMesh();
    }

    private void Start() {
        splineSampler = GetComponent<SplineSampler>();
    }

    private void OnDrawGizmos() {
        if (!Application.isPlaying)
            return;

        for (int i = 0; i < vertsP1.Count; i++) {
            Handles.SphereHandleCap(0, vertsP1[i], Quaternion.identity, 0.5f, EventType.Repaint);
            Handles.SphereHandleCap(0, vertsP2[i], Quaternion.identity, 0.5f, EventType.Repaint);
            Handles.DrawLine(vertsP1[i], vertsP2[i]);
        }

    }

    private void SetVerts() {
        vertsP1 = new List<Vector3>();
        vertsP2 = new List<Vector3>();

        float step = 1f / (float)resolution;
        for (int i = 0; i < resolution; i++) {
            float t = step * i;
            splineSampler.SampleSplineWidth(t, width, out Vector3 p1, out Vector3 p2);
            vertsP1.Add(p1);
            vertsP2.Add(p2);
        }

    }

    private void BuildMesh() {
        //MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh m = new();
        //Mesh m = gameObject.AddComponent<MeshFilter>().mesh;
        List<Vector3> verts = new();
        List<int> tris = new();
        int numberOfVerts = vertsP1.Count;

        for (int i = 1; i <= numberOfVerts; i++) {
            Vector3 p1 = vertsP1[i - 1];
            Vector3 p2 = vertsP2[i - 1];
            Vector3 p3;
            Vector3 p4;

            if (i == numberOfVerts) {
                p3 = vertsP1[0];
                p4 = vertsP2[0];
            }
            else {
                p3 = vertsP1[i];
                p4 = vertsP2[i];
            }

            int offset = 4 * (i - 1);
            int t1 = offset + 0;
            int t2 = offset + 2;
            int t3 = offset + 3;

            int t4 = offset + 3;
            int t5 = offset + 1;
            int t6 = offset + 0;

            verts.AddRange(new List<Vector3>() { p1, p2, p3, p4 });
            tris.AddRange(new List<int>() { t1, t2, t3, t4, t5, t6 });

        }

        m.SetVertices(verts);
        m.SetTriangles(tris, 0);

        meshFilter.mesh = m;
    }

}
