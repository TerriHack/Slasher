using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    private LevelGenerator levelGenerator;
    public void OnEnable()
    {
        levelGenerator = (LevelGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Door Points"))
        {
            levelGenerator.GenerateDoorSpawnPoints();
        }

        if (GUILayout.Button("Generate Level"))
        {
            levelGenerator.GenerateLevel();
        }

        if (GUILayout.Button("Clean Everything"))
        {
            levelGenerator.CleanEverything();
        }
    }
}
