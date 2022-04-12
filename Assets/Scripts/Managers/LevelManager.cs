using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField]
    private GameObject loadingCanvas;

    [SerializeField]
    private Image progressBar;

    private bool isLoading;

    private float progressTarget;

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
        }
    }

    public void LoadSceneSync(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public async void LoadSceneAsync(string sceneName)
    {
        progressTarget = 0f;
        progressBar.fillAmount = 0f;
        var scene = SceneManager.LoadSceneAsync(sceneName);

        scene.allowSceneActivation = false;

        loadingCanvas.SetActive(true);

        isLoading = true;

        do
        {
            await Task.Delay(100);
            progressTarget = scene.progress;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        isLoading = false;
        
        await Task.Delay(500);


        loadingCanvas.SetActive(false);
    }

    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, progressTarget, Time.deltaTime * 3);
    }
}