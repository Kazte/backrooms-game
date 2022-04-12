using System;
using UnityEngine;
using Discord;
using UnityEditor;

public class DiscordManager : MonoBehaviour
{
    public static DiscordManager Instance;

    public static Discord.Discord DiscordClient;

    public ActivityManager ActivityManager;

    public Activity Activity;

    public bool DiscordOpen;



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

        return;
        


        foreach (var process in System.Diagnostics.Process.GetProcesses())
        {
            if (process.ToString() == "System.Diagnostics.Process (Discord)")
            {
                DiscordOpen = true;
                break;
            }
        }

        if (DiscordOpen)
        {
            DiscordClient = new Discord.Discord(959859486586179666, (System.UInt64)Discord.CreateFlags.Default);
            ActivityManager = DiscordClient.GetActivityManager();


            Application.quitting += Dispose;
        }
    }

    public void Dispose()
    {
        return;
        if (DiscordClient != null)
        {
            DiscordClient.Dispose();
        }
    }

    public void SetActivity(Activity activity)
    {
        return;
        if (!DiscordOpen) return;

        Activity = activity;

        ActivityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                Debug.LogError("Everything is fine!");
            }
        });
    }


    // Update is called once per frame
    void Update()
    {
        return;
        if (!DiscordOpen) return;

        DiscordClient.RunCallbacks();
    }
}