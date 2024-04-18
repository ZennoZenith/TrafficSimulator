using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphGenerator))]
public class GraphGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        DrawDefaultInspector();

        GraphGenerator graphGenerator = (GraphGenerator)target;

        if (GUILayout.Button("Regenerate Graph")) {
            graphGenerator.GenerateGraph();
        }

    }
}
