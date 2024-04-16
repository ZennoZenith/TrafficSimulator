using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoadMeshGenerator))]
public class RoadMeshGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        RoadMeshGenerator roadMeshGenerator = (RoadMeshGenerator)target;

        if (GUILayout.Button("Regenerate road mesh")) {
            roadMeshGenerator.Rebuild();
        }

    }
}
