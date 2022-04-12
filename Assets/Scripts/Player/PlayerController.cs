using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rigidbody;

    private AudioSource audioSource;

    private Health health;
    public bool CanBeHealed => health.CanBeHealed;

    [SerializeField]
    private float walkSpeed = 5f;

    [SerializeField]
    private float runSpeed = 8f;

    private float currentSpeed;

    private int currentMoney;

    private Vector3 movement;

    private float hInput, vInput;

    [SerializeField]
    private float maxStamina;

    [SerializeField]
    private float staminaLoseMulti = 1f;

    [SerializeField]
    private float staminaRecoveryMulti = 1f;

    private float currentStamina;

    [SerializeField]
    private List<AudioClip> footstepAudioClip;

    [SerializeField]
    private AudioClip oofAudioClip;

    [Header("Head Bob")]
    private bool headBobEnable = true;

    [SerializeField]
    private float headBobAmplitude = 0.015f;


    [SerializeField]
    private float headBobFreq = 10f;

    [SerializeField]
    private Transform cameraHolder;

    private float headBobToggleSpeed = 3f;

    private Vector3 headBobStartPosition;

    public bool isTired;

    private InputManager inputManager;

    public bool Freeze;

    [SerializeField]
    private float interactableDistance = 1f;

    [SerializeField]
    private Interactable currentInteractable;

    private Camera camera;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        health = GetComponent<Health>();

        camera = Camera.main;


        inputManager = InputManager.Instance;

        // Events
        inputManager.OnMove += move =>
        {
            if (Freeze) return;

            hInput = move.x;
            vInput = move.y;
        };

        inputManager.OnRun += b =>
        {
            if (Freeze) return;

            isRunning = b;
        };

        inputManager.OnPause += () =>
        {
            if (HudManager.Instance.IsPause)
            {
                HudManager.Instance.ResumeGame();
            }
            else
            {
                HudManager.Instance.PauseGame();
            }
        };

        inputManager.OnInteract += b =>
        {
            if (Freeze) return;

            if (currentInteractable != null)
                currentInteractable.OnInteract(this);
        };

        health.OnDamage += Health_OnDamage;

        health.OnHeal += Health_OnHeal;

        health.OnDeath += Health_OnDeath;

        headBobStartPosition = cameraHolder.localPosition;
    }
    private void Health_OnDamage(DamageSource obj)
    {
        HudManager.Instance.SetHPBar(health.GetHealthPercent());
        audioSource.PlayOneShot(oofAudioClip);
    }

    private void Health_OnDeath(DamageSource obj)
    {
        HudManager.Instance.ShowDeath();
    }

    private void Health_OnHeal()
    {
        HudManager.Instance.SetHPBar(health.GetHealthPercent());
    }

    private void Start()
    {
        currentSpeed = walkSpeed;
        currentStamina = maxStamina;
        HudManager.Instance.SetMoney(0);

        StartCoroutine(PlayFootstep());

        HudManager.Instance.SetHPBar(1);
    }

    private void Update()
    {
        if (Freeze) return;

        GetInputs();

        HudManager.Instance.SetStamina(currentStamina / maxStamina);

        if (headBobEnable)
        {
            CheckHeadBobMotion();
            ResetPosition();
        }

        var ray = new Ray(camera.transform.position, camera.transform.forward);

        if (Physics.Raycast(ray, out var hit, interactableDistance))
        {
            if (hit.collider.gameObject.TryGetComponent(out Interactable interactable))
            {
                currentInteractable = interactable;
                HudManager.Instance.ShowInteractable(interactable.InteractText);
            }
        }
        else
        {
            HudManager.Instance.HideInteractable();
            currentInteractable = null;
        }
    }
    private void CheckHeadBobMotion()
    {
        var speed = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z).magnitude;

        if (speed < headBobToggleSpeed) return;
        // Check for grounded

        PlayMotion(FootStepMotion());
    }

    private void ResetPosition()
    {
        if (cameraHolder.localPosition == headBobStartPosition)
        {
            return;
        }

        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, headBobStartPosition, 2 * Time.deltaTime);
    }

    private void PlayMotion(Vector3 motion)
    {
        cameraHolder.localPosition += motion;
    }

    private Vector3 FootStepMotion()
    {
        var pos = Vector3.zero;

        pos.y = Mathf.Sin(Time.time * (currentSpeed * 2f)) * (isRunning ? headBobAmplitude * 1.7666f : headBobAmplitude) * Time.deltaTime;
        // pos.x = Mathf.Cos(Time.time * (currentSpeed / 2f)) * headBobAmplitude;
        return pos;
    }

    private void FixedUpdate()
    {
        if (Freeze) return;

        movement = transform.right * hInput + transform.forward * vInput;
        movement.Normalize();
        movement *= currentSpeed;

        rigidbody.velocity = new Vector3(movement.x, rigidbody.velocity.y, movement.z);
    }
    private bool isRunning;
    private void GetInputs()
    {
        if (isRunning)
        {
            if (currentStamina > 0 && !isTired)
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        if (currentSpeed == runSpeed && (Mathf.Abs(hInput) > 0.1f || Mathf.Abs(vInput) > 0.1f))
        {
            if (currentStamina > 0)
                currentStamina -= Time.deltaTime * staminaLoseMulti;
            else
            {
                currentStamina = 0f;
            }
        }
        else
        {
            if (currentStamina <= maxStamina)
                currentStamina += Time.deltaTime * staminaRecoveryMulti;
            else
            {
                currentStamina = maxStamina;
            }
        }

        if (currentStamina <= 0f)
        {
            isTired = true;
            HudManager.Instance.IsTired(true);
        }
        else if (currentStamina >= maxStamina)
        {
            isTired = false;
            HudManager.Instance.IsTired(false);
        }

        headBobEnable = !HudManager.Instance.IsPause;
    }

    private IEnumerator PlayFootstep()
    {
        yield return new WaitForSeconds(1f / (currentSpeed * 0.3f));

        if (movement.magnitude > 0)
        {
            audioSource.PlayOneShot(footstepAudioClip[Random.Range(0, footstepAudioClip.Count)]);
        }
        StartCoroutine(PlayFootstep());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hurt"))
        {
            health.Damage(new DamageSource
            {
                Damage = Random.Range(4, 8)
            });
        }
    }


    public void Heal(int heal)
    {
        health.Heal(heal);
    }
    public void AddMoney(int money)
    {
        currentMoney += money;
        HudManager.Instance.SetMoney(currentMoney);
    }

    public bool SubMoney(int money)
    {
        var targetMoney = currentMoney - money;

        if (targetMoney < 0)
        {
            currentMoney = 0;
            return false;
        }
        else
        {
            currentMoney = targetMoney;
        }
        HudManager.Instance.SetMoney(currentMoney);
        return true;
    }

    // DEBUG

    [ContextMenu("Debug_AddMoney")]
    public void Debug_AddMoney()
    {
        AddMoney(10000);
    }
}