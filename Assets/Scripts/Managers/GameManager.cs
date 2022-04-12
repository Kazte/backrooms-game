using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    MainMenu,
    InGame,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private AudioSource audioSource;

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (SceneManager.GetActiveScene().name == "Managers")
                LevelManager.Instance.LoadSceneSync("Menu");
        }
        else
        {
            Destroy(gameObject);
        }


        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayAudioOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // LOAD SCENES

    //
    // public void LoadGame()
    // {
    //     loadingScreen.gameObject.SetActive(true);
    //     
    //     scenesLoading.Add(SceneManager.UnloadSceneAsync("Menu"));
    //     scenesLoading.Add(SceneManager.LoadSceneAsync("Game"));
    //     StartCoroutine(GetSceneLoadingProgress());
    // }
    //
    // public void ReturnToMainMenu()
    // {
    //     loadingScreen.gameObject.SetActive(true);
    //     
    //     // scenesLoading.Add(SceneManager.UnloadSceneAsync("Game"));
    //     scenesLoading.Add(SceneManager.LoadSceneAsync("Menu"));
    //
    //     StartCoroutine(GetSceneLoadingProgress());
    // }
    //
    //
    // [SerializeField]
    // private Image progressBar;
    //
    // private float totalSceneProgress;
    //
    // public IEnumerator GetSceneLoadingProgress()
    // {
    //     for (int i = 0; i < scenesLoading.Count; i++)
    //     {
    //         while (!scenesLoading[i].isDone)
    //         {
    //             Debug.Log(scenesLoading[i]);
    //             totalSceneProgress = 0f;
    //
    //             foreach (var operation in scenesLoading)
    //             {
    //                 totalSceneProgress += operation.progress;
    //             }
    //
    //             totalSceneProgress = (totalSceneProgress / scenesLoading.Count);
    //
    //             progressBar.fillAmount = totalSceneProgress;
    //             
    //             
    //
    //             yield return null;
    //         }
    //     }
    //
    //     loadingScreen.gameObject.SetActive(false);
    //     scenesLoading.Clear();
    // }
}