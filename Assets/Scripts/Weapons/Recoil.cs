using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Recoil : MonoBehaviour
{
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField]
    private RecoilData currentRecoilData;

    [SerializeField]
    private float snappiness;

    [SerializeField]
    private float returnSpeed;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private Transform head;



    private void Awake()
    {
        cameraController = GetComponentInChildren<CameraController>();
    }

    private float rotAroundX;
    
    private void LateUpdate()
    {
        
        
        targetRotation = Vector3.Lerp(targetRotation, new Vector3(0f, 0f, 0f), returnSpeed * Time.deltaTime);

        currentRotation = Vector3.Slerp(currentRotation, new Vector3(targetRotation.x, targetRotation.y, targetRotation.z), snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void SetRecoilData(RecoilData recoilData) => currentRecoilData = recoilData;

    public void RecoilFire()
    {
        targetRotation += new Vector3(currentRecoilData.recoiX, Random.Range(-currentRecoilData.recoiY, currentRecoilData.recoiY), Random.Range(-currentRecoilData.recoiZ, currentRecoilData.recoiZ));
    }
}

[Serializable]
public class RecoilData
{
    [SerializeField]
    internal float recoiX;

    [SerializeField]
    internal float recoiY;

    [SerializeField]
    internal float recoiZ;
}