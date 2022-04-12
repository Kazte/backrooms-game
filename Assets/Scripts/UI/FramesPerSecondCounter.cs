using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramesPerSecondCounter : MonoBehaviour
{
    private float _deltaTime;

    [SerializeField]
    private Font font;

    

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle
        {
            alignment = TextAnchor.UpperRight,
            fontSize = Mathf.CeilToInt(h * 2.5f / 100f),
            font = font,
            normal = {textColor = new Color(1f, 1f, 1f, 1f)},
        };
        
        Rect rect = new Rect(5f, 5f, w - 5f, h * 2.5f / 100f);

        var ms = _deltaTime * 1000f;
        var fps = 1f / _deltaTime;
        var text = $"{ms:0.0} ms ({fps:0.} fps)";

        GUI.Label(rect, text, style);
    }
}