using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public List<Spawner> spawners;

    public bool CanSpawn;

    private int currentEnemiesSpawned;

    private int maxEnemiesSpawned = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public int WaveCount;

    public float timeLeftToNextWave;

    public bool isEndWave;

    [SerializeField]
    private AudioClip[] newWaveClip;

    [SerializeField]
    private AudioClip bgMusic;

    public event Action OnStartWave;
    private void Start()
    {
        spawners = FindObjectsOfType<Spawner>().ToList();

        foreach (var spawner in spawners)
        {
            spawner.OnEnemySpawn += () => { currentEnemiesSpawned++; };
        }

        CanSpawn = true;


        StartNewWave();
        
        GameManager.Instance.ChangeMusic(bgMusic);
    }

    private void Update()
    {
        if (currentEnemiesSpawned >= maxEnemiesSpawned)
        {
            CanSpawn = false;
        }

        var isAllEnemiesDead = true;
        foreach (var spawner in spawners)
        {
            if (spawner.EnemiesSpawned.Count > 0)
            {
                isAllEnemiesDead = false;
            }
        }

        if (isAllEnemiesDead && !isEndWave && !CanSpawn)
        {
            timeLeftToNextWave = 60f;
            EndWave();
        }

        if (isEndWave)
        {
            timeLeftToNextWave -= Time.deltaTime;
            HudManager.Instance.SetTimeLeft(timeLeftToNextWave);
            if (timeLeftToNextWave <= 0f)
            {
                StartNewWave();
                isEndWave = false;
            }
        }
    }

    public void StartNewWave()
    {
        var wave = WaveCount + 1;
        SetWaveCount(wave);
        HudManager.Instance.HideTimeLeft();

        var pow = Mathf.Pow(WaveCount, 5f) * 5;
        var log = Mathf.Log(pow, 3);
        var abs = Mathf.Abs(log);
        
        // =abs(log(3,x^(5)))+10;
        
        maxEnemiesSpawned = Mathf.FloorToInt(abs+5); 
        
        currentEnemiesSpawned = 0;
        
        CanSpawn = true;
        
        OnStartWave?.Invoke();
        
        GameManager.Instance.PlayAudioOneShot(newWaveClip[Random.Range(0, newWaveClip.Length)]);
    }

    public void EndWave()
    {
        HudManager.Instance.ShowTimeLeft(timeLeftToNextWave);
        isEndWave = true;
    }

    private void SetWaveCount(int wave)
    {
        WaveCount = wave;
        HudManager.Instance.SetWaveCount(WaveCount);
    }
}