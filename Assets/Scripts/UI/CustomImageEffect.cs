﻿using UnityEngine;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class CustomImageEffect : MonoBehaviour
{
    public Material _material;


    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_material != null)
        {
            Graphics.Blit(src, dest, _material);
        }
    }
}