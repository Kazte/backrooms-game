using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private Camera camera;
    
    private void Awake()
    {
        camera = Camera.main;
    }
    private void Update()
    {
        var look = camera.transform;
        transform.LookAt(look);
    }
}
