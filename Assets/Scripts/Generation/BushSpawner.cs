using UnityEngine;

public class BushSpawner : MonoBehaviour
{
    [Header("References")]
    public WorldSettings settings;
    public GameObject[] bushPrefabs;
    public Transform bushesParent;

    [Header("Spawn Settings")]
    [Min(0)]
    public int spawnAttempts = 900;

    [Range(0f, 1f)]
    public float grassSpawnChance = 0.18f;

    [Range(0f, 1f)]
    public float forestSpawnChance = 0.28f;

    [Header("Placement")]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 25f;

    public float minimumScale = 0.7f;
    public float maximumScale = 1.3f;

    [Header("Resources")]
    [Range(0f, 1f)]
    public float foodBushChance = 1f;

    public int minimumFoodDrop = 1;
    public int maximumFoodDrop = 3;

    public void GenerateBushes()
    {
        if (settings == null)
        {
            Debug.LogError("BushSpawner: WorldSettings is missing.");
            return;
        }

        if (bushPrefabs == null || bushPrefabs.Length == 0)
        {
            Debug.LogError("BushSpawner: No bush prefabs assigned.");
            return;
        }

        if (bushesParent == null)
        {
            bushesParent = transform;
        }

        ClearExistingBushes();

        Random.InitState(settings.seed + 30000);

        float worldWidth =
            settings.chunkSize *
            settings.chunksX *
            settings.vertexSpacing;

        float worldDepth =
            settings.chunkSize *
            settings.chunksZ *
            settings.vertexSpacing;

        int spawnedBushes = 0;

        for (int i = 0; i < spawnAttempts; i++)
        {
            float randomX = Random.Range(0f, worldWidth);
            float randomZ = Random.Range(0f, worldDepth);

            Vector3 rayOrigin = new Vector3(
                randomX,
                raycastHeight,
                randomZ
            );

            if (!Physics.Raycast(
                    rayOrigin,
                    Vector3.down,
                    out RaycastHit hit,
                    raycastHeight * 2f))
            {
                continue;
            }

            if (hit.collider.GetComponent<TerrainChunk>() == null)
            {
                continue;
            }

            float normalizedHeight =
                Mathf.Clamp01(
                    hit.point.y / settings.heightMultiplier
                );

            BiomeType biome =
                BiomeGenerator.GetBiome(normalizedHeight);

            float spawnChance;

            if (biome == BiomeType.Grass)
            {
                spawnChance = grassSpawnChance;
            }
            else if (biome == BiomeType.Forest)
            {
                spawnChance = forestSpawnChance;
            }
            else
            {
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

            SpawnBush(hit.point, spawnedBushes);
            spawnedBushes++;
        }

        Debug.Log(
            $"BushSpawner generated {spawnedBushes} bushes."
        );
    }

    private void SpawnBush(Vector3 position, int index)
    {
        GameObject prefab =
            bushPrefabs[Random.Range(0, bushPrefabs.Length)];

        Quaternion rotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f
        );

        GameObject bush = Instantiate(
            prefab,
            position,
            rotation,
            bushesParent
        );

        bush.name = $"Bush_{index:0000}";

        float randomScale =
            Random.Range(minimumScale, maximumScale);

        bush.transform.localScale *= randomScale;

        ResourceNode node = bush.GetComponent<ResourceNode>();

        if (node == null)
        {
            node = bush.AddComponent<ResourceNode>();
        }

        node.Initialize(
            ResourceKind.Food,
            1,
            Random.Range(
                minimumFoodDrop,
                maximumFoodDrop + 1
            )
        );

        BushInteraction interaction =
            bush.GetComponent<BushInteraction>();

        if (interaction == null)
        {
            bush.AddComponent<BushInteraction>();
        }
    }
    
    private void ClearExistingBushes()
    {
        for (int i = bushesParent.childCount - 1; i >= 0; i--)
        {
            GameObject child =
                bushesParent.GetChild(i).gameObject;

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