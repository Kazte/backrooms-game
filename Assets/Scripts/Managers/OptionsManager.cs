using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        crosshairList = Resources.LoadAll<CrosshairData>("CrossHair").ToList();

        LoadOptions();
        tabGroup.ResetAll();
        HideOption();
    }

    [SerializeField]
    private TabGroup tabGroup;

    [SerializeField]
    private GameObject optionPanel;

    [Header("Audio Options")]
    [SerializeField]
    private AudioMixer masterAudioMixer;

    [SerializeField]
    private Slider masterSlider;

    [Space(6f)]
    [SerializeField]
    private Slider musicSlider;

    [Space(6f)]
    [SerializeField]
    private Slider effectsSlider;


    #region Gameplay

    [Header("Video")]
    [SerializeField]
    private TMP_Dropdown qualitiesDropdown;

    [SerializeField]
    private TMP_Dropdown displaysDropdown;

    [SerializeField]
    private TMP_Dropdown resolutionsDropdown;

    #endregion

    #region Gameplay

    [Header("Gameplay")]
    [SerializeField]
    private List<CrosshairData> crosshairList;

    [SerializeField]
    private TMP_Dropdown crosshairDropdown;

    public CrosshairData CurrentCrosshair;

    [SerializeField]
    private Slider sensSlider;

    public float Sensitivity;

    [SerializeField]
    private Toggle toggleDebug;

    public bool ShowDebug;

    [SerializeField]
    private FramesPerSecondCounter fpsCounter;

    [SerializeField]
    private Slider fovSlider;

    [SerializeField]
    private TextMeshProUGUI fovNumberText;


    public event Action OnCrosshairChange;

    public event Action<int> OnFovChange;

    public int Fov => (int)fovSlider.value;

    #endregion

    [Space]
    public bool IsOptionOpen;



    private void Start()
    {
    }

    public void LoadOptions()
    {
        // Audio
        var masterVolume = PlayerPrefs.GetFloat("MasterVolume", 10);
        masterAudioMixer.SetFloat("MasterVol", Mathf.Log10(masterVolume.Remap(0, 10, 0.0001f, 1)) * 20f);
        masterSlider.value = masterVolume;

        var musicVolume = PlayerPrefs.GetFloat("MusicVolume", 10);
        masterAudioMixer.SetFloat("MusicVol", Mathf.Log10(musicVolume.Remap(0, 10, 0.0001f, 1)) * 20f);
        musicSlider.value = musicVolume;

        var effectsVolume = PlayerPrefs.GetFloat("EffectsVolume", 10);
        masterAudioMixer.SetFloat("EffectsVol", Mathf.Log10(effectsVolume.Remap(0, 10, 0.0001f, 1)) * 20f);
        effectsSlider.value = effectsVolume;

        // Video
        foreach (var qualitySettings in QualitySettings.names)
        {
            qualitiesDropdown.options.Add(new TMP_Dropdown.OptionData(qualitySettings));
        }
        var ql = PlayerPrefs.GetInt("QualityLevel", 2);
        SetQualitySetting(ql);
        qualitiesDropdown.SetValueWithoutNotify(ql);

        // foreach (var d in Display.main)
        // {
        //     displaysDropdown.options.Add(new TMP_Dropdown.OptionData(d.)));
        // }
        // var display = PlayerPrefs.GetInt("Display", 0);
        // SetDisplay(display);
        // displaysDropdown.SetValueWithoutNotify(display);

        SetDisplay(0);

        UpdateResolutions();
        var res = PlayerPrefs.GetInt("Resolution", Screen.resolutions.Length - 1);
        SetResolution(res);
        resolutionsDropdown.SetValueWithoutNotify(res);


        // Gameplay
        CurrentCrosshair = crosshairList[PlayerPrefs.GetInt("CrosshairIndex", 0)];
        OnCrosshairChange?.Invoke();
        foreach (var crosshairData in crosshairList)
        {
            crosshairDropdown.options.Add(new TMP_Dropdown.OptionData(crosshairData.name, crosshairData.Sprite));
        }
        HandleCrosshairDropdown(PlayerPrefs.GetInt("CrosshairIndex", 0));
        crosshairDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt("CrosshairIndex", Screen.resolutions.Length - 1));

        Sensitivity = PlayerPrefs.GetFloat("Sens", 50f);
        sensSlider.value = Sensitivity;

        ShowDebug = PlayerPrefs.GetInt("ShowDebug", 0) == 1;
        toggleDebug.isOn = ShowDebug;
        fpsCounter.enabled = ShowDebug;

        fovSlider.value = PlayerPrefs.GetInt("FOV", 60);
        fovNumberText.text = fovSlider.value.ToString();
        OnFovChange?.Invoke((int)fovSlider.value);
    }

    public void SaveOptions()
    {
        // Audio
        var masterVolume = masterSlider.value;
        masterAudioMixer.SetFloat("MasterVol", Mathf.Log10(masterVolume.Remap(0, 10, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);


        var musicVolume = musicSlider.value;
        masterAudioMixer.SetFloat("MusicVol", Mathf.Log10(musicVolume.Remap(0, 10, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        var effectsVolume = effectsSlider.value;
        masterAudioMixer.SetFloat("EffectsVol", Mathf.Log10(effectsVolume.Remap(0, 10, 0.0001f, 1)) * 20f);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolume);

        PlayerPrefs.SetFloat("Sens", Sensitivity);

        // Gameplay

        PlayerPrefs.SetInt("ShowDebug", ShowDebug ? 1 : 0);


        HideOption();
    }

    public void HandleCrosshairDropdown(int val)
    {
        CurrentCrosshair = crosshairList[val];
        OnCrosshairChange?.Invoke();
    }
    public void ShowOption()
    {
        optionPanel.SetActive(true);
        tabGroup.ResetAll();
        IsOptionOpen = true;
    }

    public void HideOption()
    {
        optionPanel.SetActive(false);
        IsOptionOpen = false;
    }

    public void SetSensitivity(Single sens)
    {
        Sensitivity = sens;
    }

    public void SetDebugToggle(bool set)
    {
        ShowDebug = set;
        fpsCounter.enabled = ShowDebug;
    }

    public void SetFov(Single fov)
    {
        var currentFov = (int)fov;

        fovNumberText.text = currentFov.ToString();

        PlayerPrefs.SetInt("FOV", currentFov);

        OnFovChange?.Invoke(currentFov);
    }

    public void SetQualitySetting(int val)
    {
        QualitySettings.SetQualityLevel(val, true);
        PlayerPrefs.SetInt("QualityLevel", val);
    }

    public void SetDisplay(int val)
    {
        Display.displays[val].Activate();
        PlayerPrefs.SetInt("Display", val);
        UpdateResolutions();
    }

    public void SetResolution(int val)
    {
        var res = Screen.resolutions[val];

        Screen.SetResolution(res.width, res.height, true);

        PlayerPrefs.SetInt("Resolution", val);
    }

    public void UpdateResolutions()
    {
        var resolutions = Screen.resolutions;
        
        resolutions = resolutions.Distinct().ToArray();

        foreach (var resolution in resolutions)
        {
            Debug.Log(resolution);
        }

        resolutionsDropdown.ClearOptions();

        foreach (var resolution in resolutions)
        {
            resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height));
        }
    }
}

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}