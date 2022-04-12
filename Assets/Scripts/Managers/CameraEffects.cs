 
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CameraShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);


            elapsed += Time.deltaTime;
            
            yield return null;
        }

        transform.localPosition = originalPos;


    }
}
