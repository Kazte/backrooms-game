using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Position")]
    [SerializeField]
    private float amount = 0.02f;

    [SerializeField]
    private float maxAmount = 0.06f;

    [SerializeField]
    private float smoothAmount = 6f;

    [Header("Rotation")]
    [SerializeField]
    private float amountRotation = 4f;

    [SerializeField]
    private float maxAmountRotation = 5f;

    [SerializeField]
    private float smoothAmountRotation = 12f;

    [Space]
    [SerializeField]
    private bool rotationX = true;

    [SerializeField]
    private bool rotationY = true;

    [SerializeField]
    private bool rotationZ = true;


    private Vector3 initialPos;
    private Quaternion initialRotation;

    private float inputX;
    private float inputY;

    private InputManager inputManager;

    private PlayerController playerController;



    private void Awake()
    {
        inputManager = InputManager.Instance;

        inputManager.OnLook += look =>
        {
            inputX = -look.x * (OptionsManager.Instance.Sensitivity * (1f / 100f));
            inputY = -look.y * (OptionsManager.Instance.Sensitivity * (1f / 100f));
        };

        playerController = GetComponentInParent<PlayerController>();
    }
    private void Start()
    {
        // initialPos = transform.localPosition;
        initialPos = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        if (playerController.Freeze) return;

        MoveSway();
        TiltSway();
    }

    private void TiltSway()
    {
        var tiltY = Mathf.Clamp(inputX * amountRotation, -maxAmountRotation, maxAmountRotation);
        var tiltX = Mathf.Clamp(inputY * amountRotation, -maxAmountRotation, maxAmountRotation);
        var finalRotation = Quaternion.Euler(new Vector3(
            rotationX ? -tiltX : 0f,
            rotationY ? tiltY : 0f,
            rotationZ ? tiltY : 0f
        ));


        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothAmountRotation);
    }

    private void MoveSway()
    {
        var moveX = Mathf.Clamp(inputX * amount, -maxAmount, maxAmount);
        var moveY = Mathf.Clamp(inputY * amount, -maxAmount, maxAmount);
        var finalPos = new Vector3(moveX, moveY, 0f);

        transform.localPosition =
            Vector3.Lerp(transform.localPosition, finalPos + initialPos, Time.deltaTime * smoothAmount);
    }
}