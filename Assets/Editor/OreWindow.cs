using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System;

public class OreWindow : EditorWindow
{
    [MenuItem("Window/Ore Window")]
    public static void ShowWindow()
    {
        GetWindow<OreWindow>("Ore Window");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Update Ore IDs"))
        {
            FunctionToRun();
        }
    }

    private void FunctionToRun()
    {
        Ore[] ores = FindObjectsOfType<Ore>();
        foreach (Ore o in ores)
        {
            EditorUtility.SetDirty(o);
            if (o.myGuid != "")
            {
                continue;
            }
            o.myGuid = Guid.NewGuid().ToString();
        }
    }
}
