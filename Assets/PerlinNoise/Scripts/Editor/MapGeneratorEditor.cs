using UnityEngine;
using UnityEditor;
using System.Collections;
using PerlinNoise;

[CustomEditor(typeof(PerlinNoise.MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PerlinNoise.MapGenerator mapGen = (PerlinNoise.MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if(GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
    }

}
