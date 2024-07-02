using UnityEngine;
using UnityEditor;

public class ScreenshotWindow : EditorWindow
{
    string path;
    string fileName;

    [MenuItem("Window/Screenshot")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotWindow>("Screenshot Window");
    }


    private void OnGUI()
    {
        GUILayout.Label("Screenshot Window", EditorStyles.boldLabel);

        path = EditorGUILayout.TextField("Path", path);
        fileName = EditorGUILayout.TextField("File Name", fileName);
        GUILayout.Space(20f);
        if (GUILayout.Button("Take Screenshot"))
        {
            if (fileName != "" && path != "")
            {
                ScreenCapture.CaptureScreenshot(path+fileName);
            }
        }
    }
}
