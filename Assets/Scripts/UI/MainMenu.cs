using System;
using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
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
            State = "In Menu",
            Timestamps =
            {
                Start = 0
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

    public void StartGame()
    {
        // GameManager.Instance.LoadGame();
        LevelManager.Instance.LoadSceneAsync("Game");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        // DiscordManager.Instance.Dispose();
        Debug.Log("lol");
        
        Application.Quit();
    }

    public void ShowOptions()
    {
        OptionsManager.Instance.ShowOption();
    }
}