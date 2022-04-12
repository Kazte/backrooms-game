using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private float spawnTime = 5f;

    [SerializeField]
    private Enemy enemyToSpawn;

    [SerializeField]
    private float randomTime = 2f;

    private float currentSpawnTime;

    public event Action OnEnemySpawn;

    private PlayerController playerController;

    [SerializeField]
    private LayerMask detectLayers;

    private ObjectPool<Enemy> pool;

    public List<Enemy> EnemiesSpawned;

    private void Start()
    {
        currentSpawnTime = spawnTime + Random.Range(-randomTime, randomTime);

        playerController = FindObjectOfType<PlayerController>();

        pool = new ObjectPool<Enemy>(
            () =>
            {
                var enemySpawned = SpawnEnemy();

                currentSpawnTime = spawnTime + Random.Range(-randomTime, randomTime);

                return enemySpawned;
            },
            enemy =>
            {
                enemy.gameObject.SetActive(true);
                OnEnemySpawn?.Invoke();
                enemy.Init(transform.position, KillEnemy);
                if (!EnemiesSpawned.Contains(enemy)) EnemiesSpawned.Add(enemy);
                currentSpawnTime = spawnTime + Random.Range(-randomTime, randomTime);
            },
            enemy => { enemy.gameObject.SetActive(false); }, enemy => { Destroy(enemy.gameObject); });
    }

    private void KillEnemy(Enemy enemy)
    {
        pool.Release(enemy);
        
    }

    private void Update()
    {
        if (!WaveManager.Instance.CanSpawn)
            return;

        if (currentSpawnTime <= 0)
        {
            var dir =
                (playerController.transform.position + Vector3.up * 0.5f) - transform.position;
            var dis = Vector3.Distance(transform.position, playerController.transform.position);

            var seen = false;
            var canSpawn = false;

            for (int i = -5; i <= 5; i++)
            {
                if (
                    !Physics.Raycast(
                        transform.position,
                        dir + Vector3.right * ((i * 0.5f) / dis),
                        out var hit,
                        float.MaxValue,
                        detectLayers
                    )
                )
                    return;

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    // Debug.Log(gameObject.name + " is trying to spawn, but is looking the player.");
                    currentSpawnTime = spawnTime + Random.Range(-randomTime, randomTime);
                    seen = true;
                }

                if (i == 5 && !seen)
                {
                    canSpawn = true;
                }
            }

            if (canSpawn)
            {
                // SpawnEnemy();
                pool.Get();
            }
        }
        else
        {
            currentSpawnTime -= Time.deltaTime;
        }
    }

    private Enemy SpawnEnemy()
    {
        var e = Instantiate(enemyToSpawn, transform.position, Quaternion.identity, transform);
        

        EnemiesSpawned.Add(e);
        e.OnDeath += enemy =>
        {
            EnemiesSpawned.Remove(enemy);
        };

        return e;
    }
}