using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [Header("References")]
    public WorldSettings settings;
    public GameObject[] rockPrefabs;
    public Transform rocksParent;

    [Header("Spawn Settings")]
    [Min(0)]
    public int spawnAttempts = 900;

    [Range(0f, 1f)]
    public float sandSpawnChance = 0.04f;

    [Range(0f, 1f)]
    public float grassSpawnChance = 0.08f;

    [Range(0f, 1f)]
    public float rockBiomeSpawnChance = 0.40f;

    [Header("Placement")]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 35f;

    public float minimumScale = 0.7f;
    public float maximumScale = 1.6f;

    public void GenerateRocks()
    {
        if (settings == null)
        {
            Debug.LogError("RockSpawner: WorldSettings is missing.");
            return;
        }

        if (rockPrefabs == null || rockPrefabs.Length == 0)
        {
            Debug.LogError("RockSpawner: No rock prefabs assigned.");
            return;
        }

        if (rocksParent == null)
        {
            rocksParent = transform;
        }

        ClearExistingRocks();

        Random.InitState(settings.seed + 20000);

        float worldWidth =
            settings.chunkSize *
            settings.chunksX *
            settings.vertexSpacing;

        float worldDepth =
            settings.chunkSize *
            settings.chunksZ *
            settings.vertexSpacing;

        int spawnedRocks = 0;

        for (int i = 0; i < spawnAttempts; i++)
        {
            float randomX = Random.Range(0f, worldWidth);
            float randomZ = Random.Range(0f, worldDepth);

            Vector3 rayOrigin = new Vector3(
                randomX,
                raycastHeight,
                randomZ
            );

            bool hitSomething = Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                raycastHeight * 2f
            );

            if (!hitSomething)
            {
                continue;
            }

            if (hit.collider.GetComponent<TerrainChunk>() == null)
            {
                continue;
            }

            float normalizedHeight =
                hit.point.y / settings.heightMultiplier;

            normalizedHeight = Mathf.Clamp01(normalizedHeight);

            BiomeType biome =
                BiomeGenerator.GetBiome(normalizedHeight);

            float spawnChance;

            switch (biome)
            {
                case BiomeType.Sand:
                    spawnChance = sandSpawnChance;
                    break;

                case BiomeType.Grass:
                    spawnChance = grassSpawnChance;
                    break;

                case BiomeType.Rock:
                    spawnChance = rockBiomeSpawnChance;
                    break;

                default:
                    continue;
            }

            if (Random.value > spawnChance)
            {
                continue;
            }

            float slopeAngle =
                Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle > maximumSlope)
            {
                continue;
            }

            SpawnRock(hit.point, spawnedRocks);
            spawnedRocks++;
        }

        Debug.Log(
            $"RockSpawner generated {spawnedRocks} rocks."
        );
    }

    private void SpawnRock(Vector3 position, int index)
    {
        GameObject prefab =
            rockPrefabs[Random.Range(0, rockPrefabs.Length)];

        Quaternion rotation = Quaternion.Euler(
            Random.Range(-8f, 8f),
            Random.Range(0f, 360f),
            Random.Range(-8f, 8f)
        );

        GameObject rock = Instantiate(
            prefab,
            position,
            rotation,
            rocksParent
        );

        rock.name = $"Rock_{index:0000}";

        float randomScale =
            Random.Range(minimumScale, maximumScale);

        rock.transform.localScale *= randomScale;
        
        ResourceNode node = rock.GetComponent<ResourceNode>();

        if (node == null)
        {
            node = rock.AddComponent<ResourceNode>();
        }

        node.resourceKind = ResourceKind.Stone;
        node.maxHealth = 4;
        node.dropAmount = Random.Range(2, 5);
    }

    private void ClearExistingRocks()
    {
        for (int i = rocksParent.childCount - 1; i >= 0; i--)
        {
            GameObject child =
                rocksParent.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(child);
            }
            else
            {
                DestroyImmediate(child);
            }
        }
    }
}