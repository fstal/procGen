using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        MeshGenerator meshGen = (MeshGenerator)target;

        if (DrawDefaultInspector()) 
        {
            if (meshGen.autoUpdate) meshGen.Render();
        }
        if (GUILayout.Button ("Render")) meshGen.Render();
        if (GUILayout.Button ("Erode")) meshGen.runErosion(false);
    }
}
