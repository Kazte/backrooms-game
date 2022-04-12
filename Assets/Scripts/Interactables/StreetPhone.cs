using System;
using UnityEngine;

public class StreetPhone : Interactable
{
    private AudioSource audioSource;

    [SerializeField]
    private GameObject shop;

    private PlayerController playerController;

    private bool isShopOpen;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        playerController = FindObjectOfType<PlayerController>();

        WaveManager.Instance.OnStartWave += CloseStore;
    }

    public override void OnInteract(PlayerController playerController)
    {
        if (!WaveManager.Instance.isEndWave) return;

        // Show Shop
        Cursor.lockState = CursorLockMode.Confined;
        isShopOpen = true;
        this.playerController = playerController;
        playerController.Freeze = true;
        shop.gameObject.SetActive(true);
        HudManager.Instance.IsEnded = true;
    }

    public void CloseStore()
    {
        shop.gameObject.SetActive(false);
        isShopOpen = false;
        playerController.Freeze = false;
        Cursor.lockState = CursorLockMode.Locked;

        HudManager.Instance.IsEnded = false;
    }

    private void Update()
    {
        if (WaveManager.Instance.isEndWave)
        {
            if (!audioSource.isPlaying)
            {
                if (HudManager.Instance.IsPause)
                {
                    audioSource.Pause();
                }
                else
                {
                    audioSource.Play();
                }
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void NextWave()
    {
        WaveManager.Instance.timeLeftToNextWave = 1f;
        CloseStore();
    }
}