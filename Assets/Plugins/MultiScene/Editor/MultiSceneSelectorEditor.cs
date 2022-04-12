using System;
using UnityEditor;
using UnityEngine;

public class MultiSceneSelectorEditor : EditorWindow
{
    [MenuItem("Tools/Multi Scene Selector")]
    public static void ShowWindow()
    {
        GetWindow<MultiSceneSelectorEditor>("Multi Scene Selector");
    }

    private void OnGUI()
    {
        var multiSceneResources = Resources.LoadAll<MultiScene>("Scene Groups");
        EditorGUILayout.Space(16f);
        
        foreach (var multiScene in multiSceneResources)
        {
            if (multiScene)
            {
                if (GUILayout.Button(multiScene.name))
                {
                    AssetDatabase.OpenAsset(multiScene);
                }
            }
        }
    }
}