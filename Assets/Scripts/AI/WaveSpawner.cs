using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public DayNightCycle dayNightCycle;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    public int baseEnemiesPerWave = 2;

    private int dayNumber = 1;
    private bool spawnedThisNight;

    private void Update()
    {
        if (dayNightCycle == null || enemyPrefab == null || spawnPoints.Length == 0)
            return;

        if (dayNightCycle.IsNight && !spawnedThisNight)
        {
            SpawnWave();
            spawnedThisNight = true;
        }

        if (!dayNightCycle.IsNight && spawnedThisNight)
        {
            spawnedThisNight = false;
            dayNumber++;
        }
    }

    private void SpawnWave()
    {
        int enemiesToSpawn = baseEnemiesPerWave + dayNumber;

        Debug.Log($"Night wave started! Spawning {enemiesToSpawn} enemies.");

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}