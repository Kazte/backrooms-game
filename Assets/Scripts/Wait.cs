 
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Wait : MonoBehaviour
{

    [SerializeField]
    private float waitTime;
    
    private void Start()
    {
        StartCoroutine(WaitTime());
    }

    private void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Managers");
            
            StopCoroutine(WaitTime());
        }
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene("Managers");
    }
}
