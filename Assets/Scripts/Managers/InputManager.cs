using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        Instance = this;
        SetControls();
    }

    private InputControls inputControls;

    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action<bool> OnRun;
    public event Action<KeyInputType> OnShoot;
    public event Action<bool> OnReload;
    public event Action<float> OnWeaponSwitch;
    public event Action<bool> OnInteract;
    public event Action OnPause;
    private void SetControls()
    {
        inputControls ??= new InputControls();
        inputControls.Enable();

        inputControls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());

        inputControls.Player.Look.performed += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>() / 4f);

        inputControls.Player.Run.performed += ctx => OnRun?.Invoke(ctx.ReadValueAsButton());

        inputControls.Player.Reload.performed += ctx => OnReload?.Invoke(ctx.ReadValueAsButton());

        inputControls.Player.Interact.performed += ctx => OnInteract?.Invoke(ctx.ReadValueAsButton());

        inputControls.Player.WeaponSwitch.performed += ctx => OnWeaponSwitch?.Invoke(ctx.ReadValue<float>());

        inputControls.Player.Pause.started += ctx => { OnPause?.Invoke(); };
    }

    private bool shotPressed;
    private void Update()
    {
        if (inputControls.Player.Shot.WasPressedThisFrame())
        {
            if (!shotPressed)
            {
                OnShoot?.Invoke(KeyInputType.Down);
            }
        }
        else if (inputControls.Player.Shot.IsPressed())
        {
            OnShoot?.Invoke(KeyInputType.Pressed);
            shotPressed = true;
        }
         
        else if (inputControls.Player.Shot.WasReleasedThisFrame())
        {
            OnShoot?.Invoke(KeyInputType.Up);
            shotPressed = false;
        }
    }
}

public enum KeyInputType
{
    Down,
    Pressed,
    Up
}