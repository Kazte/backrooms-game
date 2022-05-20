using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //camera public values
    [SerializeField]
    public float XMinRotation;

    [SerializeField]
    public float XMaxRotation;

    [Range(1.0f, 10.0f)]
    [SerializeField]
    private float Xsensitivity;

    [Range(1.0f, 10.0f)]
    [SerializeField]
    private float Ysensitivity;

    [SerializeField]
    private Transform body;

    [SerializeField]
    private Transform rotation;

    private PlayerController playerController;

    private float rotAroundX, rotAroundY;

    private bool camMoved = false;

    private Camera camera;

    private InputManager inputManager;

    private float x, y;

    private void Awake()
    {
        camera = GetComponent<Camera>();
        inputManager = InputManager.Instance;

        inputManager.OnLook += look =>
        {
            x = look.y;
            y = look.x;
        };

        playerController = GetComponentInParent<PlayerController>();
    }

    // Use this for initialization
    void Start()
    {
        OptionsManager.Instance.OnFovChange += fov => { camera.fieldOfView = fov; };
        camera.fieldOfView = OptionsManager.Instance.Fov;

        rotAroundX = transform.eulerAngles.x;
        rotAroundY = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (HudManager.Instance.IsPause) return;
        if (playerController.Freeze) return;



        rotAroundX += x * OptionsManager.Instance.Sensitivity.Remap(0, 100, 0, 5);
        rotAroundY += y * OptionsManager.Instance.Sensitivity.Remap(0, 100, 0, 5);

        // Clamp rotation values
        rotAroundX = Mathf.Clamp(rotAroundX, XMinRotation, XMaxRotation);
    }

    private void LateUpdate()
    {
        if (HudManager.Instance.IsPause) return;
        if (playerController.Freeze) return;

        body.transform.rotation = Quaternion.Euler(0, rotAroundY, 0);
        rotation.transform.rotation = Quaternion.Euler(-rotAroundX, rotAroundY, 0);
    }
}