using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemIconGenerator : MonoBehaviour
{
    private Camera camera;

    [SerializeField]
    private string path;

    [SerializeField]
    private List<GameObject> sceneObjects;

    private void Awake()
    {
        camera = Camera.main;
    }

    [ContextMenu("Take Screenshot")]
    private void ProcessScreenshots()
    {
        StartCoroutine(Screenshot());
    }
    
    private IEnumerator Screenshot()
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            var obj = sceneObjects[i];

            obj.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            var obj = sceneObjects[i];

            obj.gameObject.SetActive(true);

            yield return null;
            
            TakeScreenshot($"{Application.dataPath}/{path}/{obj.gameObject.name}_Icon.png");
            Debug.Log("Screenshot");

            yield return null;

            obj.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            var obj = sceneObjects[i];

            obj.gameObject.SetActive(true);
        }
    }

    private void TakeScreenshot(string fullPath)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        RenderTexture rt = new RenderTexture(256, 256, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(rt);
        }
        else
        {
            Destroy(rt);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}