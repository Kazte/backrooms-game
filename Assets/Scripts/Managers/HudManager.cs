using System;
using Discord;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance;

    [Header("HUD")]
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    [SerializeField]
    private TextMeshProUGUI ammoText;

    [SerializeField]
    private TextMeshProUGUI weaponNameText;

    [SerializeField]
    private TextMeshProUGUI waveCountText;

    [SerializeField]
    private TextMeshProUGUI timeLeftText;

    [SerializeField]
    private TextMeshProUGUI interactableText;

    [SerializeField]
    private Image crosshairImage;

    [SerializeField]
    private Image hpBar;

    [SerializeField]
    private CanvasGroup staminaGroup;

    [SerializeField]
    private Slider staminaSlider;

    [SerializeField]
    private Image vignetteImage;

    [SerializeField]
    private GameObject hudPanel;

    [Header("Pause")]
    [SerializeField]
    private GameObject pausePanel;

    [Header("Death")]
    [SerializeField]
    private GameObject deathPanel;

    [SerializeField]
    private TextMeshProUGUI scoreDeathText;

    [SerializeField]
    private AudioClip deathAudioClip;

    private int score;

    private Canvas canvas;

    public bool IsPause;

    public event Action<bool> OnPause;

    public bool IsEnded;
    private void Awake()
    {
        Instance = this;

        SetScore(0);

        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
    }

    private void Start()
    {
        if (canvas.worldCamera == null)
            canvas.worldCamera = Camera.main;

        OptionsManager.Instance.OnCrosshairChange += () => { crosshairImage.sprite = OptionsManager.Instance.CurrentCrosshair.Sprite; };

        crosshairImage.sprite = OptionsManager.Instance.CurrentCrosshair.Sprite;

        timeLeftText.gameObject.SetActive(false);

        var activity = new Activity()
        {
            Party =
            {
                Size =
                {
                    CurrentSize = 0,
                    MaxSize = 0
                }
            },
            State = $"Score: {score}",
            Timestamps =
            {
                Start = ToUnixTime()
            },
            Assets =
            {
                SmallImage = "",
                SmallText = "",
                LargeImage = "backrooms_icon_521",
                LargeText = "Backrooms"
            },
        };

        DiscordManager.Instance.SetActivity(activity);
    }

    private long ToUnixTime()
    {
        DateTime date = DateTime.UtcNow;
        var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return Convert.ToInt64((date - epoc).TotalSeconds);
    }

    public void SetAmmo(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }

    public void SetStamina(float per)
    {
        staminaSlider.value = per;
    }

    private void SetScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void SetMoney(int money)
    {
        moneyText.text = $"$ {money}";
    }

    public void PauseGame()
    {
        if (IsEnded) return;

        pausePanel.SetActive(true);
        hudPanel.SetActive(false);
        Time.timeScale = 0f;
        IsPause = true;
        OnPause?.Invoke(IsPause);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        if (IsEnded) return;

        if (pausePanel) pausePanel.SetActive(false);
        hudPanel.SetActive(true);
        Time.timeScale = 1f;
        IsPause = false;
        OnPause?.Invoke(IsPause);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        IsPause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.LoadSceneAsync("Menu");
    }

    public void SetHPBar(int current, int max)
    {
        hpBar.fillAmount = current / max;
    }

    public void SetHPBar(float percent)
    {
        hpBar.fillAmount = percent;
        vignetteImage.color = new Color(vignetteImage.color.r, vignetteImage.color.g, vignetteImage.color.b, 1 - percent);
    }
    public void SetWeaponName(string name)
    {
        weaponNameText.text = name;
    }

    public void ShowDeath()
    {
        IsPause = true;
        IsEnded = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(false);
        hudPanel.SetActive(false);
        deathPanel.SetActive(true);
        scoreDeathText.text = $"Score: {score}";
        Cursor.lockState = CursorLockMode.Confined;
        GameManager.Instance.ChangeMusic(deathAudioClip);
    }
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;

        SetScore(score);
    }

    public void OpenOptions()
    {
        OptionsManager.Instance.ShowOption();
    }
    public void IsTired(bool isTired)
    {
        staminaGroup.alpha = isTired ? 0.5f : 1f;
    }
    public void SetWaveCount(int waveCount)
    {
        waveCountText.text = $"Wave {waveCount}";
    }

    public void ShowTimeLeft(float startTime)
    {
        timeLeftText.gameObject.SetActive(true);
        var min = Mathf.FloorToInt(startTime / 60);
        var sec = Mathf.FloorToInt(startTime % 60f);
        timeLeftText.text = $"{min.ToString("D2")}:{sec.ToString("D2")}";
    }

    public void SetTimeLeft(float timeLeft)
    {
        var min = Mathf.FloorToInt(timeLeft / 60);
        var sec = Mathf.FloorToInt(timeLeft % 60f);
        timeLeftText.text = $"{min.ToString("D2")}:{sec.ToString("D2")}";
    }

    public void HideTimeLeft()
    {
        timeLeftText.gameObject.SetActive(false);
    }
    public void ShowInteractable(string interactableInteractText)
    {
        interactableText.gameObject.SetActive(true);
        interactableText.text = interactableInteractText;
    }

    public void HideInteractable()
    {
        interactableText.gameObject.SetActive(false);
    }

    public void ShowHud()
    {
        hudPanel.SetActive(true);
    }

    public void HideHud()
    {
        hudPanel.SetActive(false);
    }
}