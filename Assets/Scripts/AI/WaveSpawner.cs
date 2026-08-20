using System;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnEntry
    {
        public GameObject enemyPrefab;

        [Min(0)]
        public int weight = 100;
    }

    [Header("References")]
    public DayNightCycle dayNightCycle;
    public EnemySpawnEntry[] enemyPrefabs;
    public Transform[] spawnPoints;

    [Header("Wave Scaling")]
    [Min(0)]
    public int baseEnemiesPerWave = 3;

    [Min(0)]
    public int enemiesAddedPerNight = 1;

    [Min(1)]
    public int maximumEnemiesPerWave = 8;

    [Header("Spawn Settings")]
    [Min(1f)]
    public float spawnSearchRadius = 15f;

    [Min(1)]
    public int maxSpawnAttempts = 10;

    private int dayNumber = 1;
    private bool spawnedThisNight;

    private void Update()
    {
        if (dayNightCycle == null)
            return;

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        // Start one wave when night begins.
        if (dayNightCycle.IsNight && !spawnedThisNight)
        {
            SpawnWave();
            spawnedThisNight = true;
        }

        // Prepare for the next night.
        if (!dayNightCycle.IsNight && spawnedThisNight)
        {
            spawnedThisNight = false;
            dayNumber++;
        }
    }

    private void SpawnWave()
    {
        int enemiesToSpawn =
            baseEnemiesPerWave +
            ((dayNumber - 1) * enemiesAddedPerNight);

        enemiesToSpawn =
            Mathf.Min(
                enemiesToSpawn,
                maximumEnemiesPerWave
            );

        Debug.Log(
            $"Night {dayNumber} wave started! " +
            $"Spawning {enemiesToSpawn} enemies."
        );

        int successfullySpawned = 0;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (TrySpawnEnemy())
            {
                successfullySpawned++;
            }
        }

        Debug.Log(
            $"Night {dayNumber} wave finished. " +
            $"Spawned {successfullySpawned}/" +
            $"{enemiesToSpawn} enemies."
        );
    }

    private bool TrySpawnEnemy()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Transform spawnPoint =
                spawnPoints[
                    UnityEngine.Random.Range(
                        0,
                        spawnPoints.Length
                    )
                ];

            if (spawnPoint == null)
                continue;

            if (!NavMesh.SamplePosition(
                    spawnPoint.position,
                    out NavMeshHit hit,
                    spawnSearchRadius,
                    NavMesh.AllAreas))
            {
                continue;
            }

            GameObject enemyPrefab = GetRandomEnemyPrefab();

            if (enemyPrefab == null)
                continue;

            GameObject enemy = Instantiate(
                enemyPrefab,
                hit.position,
                spawnPoint.rotation
            );

            NavMeshAgent agent =
                enemy.GetComponent<NavMeshAgent>();

            if (agent != null && !agent.isOnNavMesh)
            {
                Debug.LogWarning(
                    $"Enemy {enemy.name} spawned " +
                    $"but is not connected to the NavMesh."
                );
            }

            return true;
        }

        Debug.LogWarning(
            "Could not find a valid NavMesh spawn position."
        );

        return false;
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int totalWeight = 0;

        foreach (EnemySpawnEntry entry in enemyPrefabs)
        {
            if (entry == null)
                continue;

            if (entry.enemyPrefab == null)
                continue;

            if (entry.weight <= 0)
                continue;

            totalWeight += entry.weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning(
                "WaveSpawner has no valid enemy spawn weights."
            );

            return null;
        }

        int randomValue =
            UnityEngine.Random.Range(0, totalWeight);

        int currentWeight = 0;

        foreach (EnemySpawnEntry entry in enemyPrefabs)
        {
            if (entry == null)
                continue;

            if (entry.enemyPrefab == null)
                continue;

            if (entry.weight <= 0)
                continue;

            currentWeight += entry.weight;

            if (randomValue < currentWeight)
            {
                return entry.enemyPrefab;
            }
        }

        return null;
    }
}