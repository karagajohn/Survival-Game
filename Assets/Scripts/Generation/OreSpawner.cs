using UnityEngine;

public class OreSpawner : MonoBehaviour
{
    [Header("References")]
    public WorldSettings settings;
    public Transform oresParent;

    [Header("Ore Prefabs")]
    public GameObject[] ironOrePrefabs;
    public GameObject[] goldOrePrefabs;

    [Header("Spawn Attempts")]
    [Min(0)]
    public int spawnAttempts = 700;

    [Header("Iron Spawn Chances")]
    [Range(0f, 1f)]
    public float ironGrassChance = 0.025f;

    [Range(0f, 1f)]
    public float ironRockChance = 0.22f;

    [Header("Gold Spawn Chances")]
    [Range(0f, 1f)]
    public float goldGrassChance = 0.002f;

    [Range(0f, 1f)]
    public float goldRockChance = 0.035f;

    [Header("Placement")]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 32f;

    public float minimumScale = 0.75f;
    public float maximumScale = 1.35f;

    public void GenerateOres()
    {
        if (settings == null)
        {
            Debug.LogError("OreSpawner: WorldSettings is missing.");
            return;
        }

        if (!HasAnyPrefabs())
        {
            Debug.LogError("OreSpawner: No ore prefabs assigned.");
            return;
        }

        if (oresParent == null)
        {
            oresParent = transform;
        }

        ClearExistingOres();

        Random.InitState(settings.seed + 40000);

        float worldWidth =
            settings.chunkSize *
            settings.chunksX *
            settings.vertexSpacing;

        float worldDepth =
            settings.chunkSize *
            settings.chunksZ *
            settings.vertexSpacing;

        int ironCount = 0;
        int goldCount = 0;

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

            float normalizedHeight = Mathf.Clamp01(
                hit.point.y / settings.heightMultiplier
            );

            BiomeType biome =
                BiomeGenerator.GetBiome(normalizedHeight);

            if (biome != BiomeType.Grass &&
                biome != BiomeType.Rock)
            {
                continue;
            }

            float slopeAngle =
                Vector3.Angle(hit.normal, Vector3.up);

            if (slopeAngle > maximumSlope)
            {
                continue;
            }

            float ironChance =
                biome == BiomeType.Rock
                    ? ironRockChance
                    : ironGrassChance;

            float goldChance =
                biome == BiomeType.Rock
                    ? goldRockChance
                    : goldGrassChance;

            float roll = Random.value;

            // Ελέγχουμε πρώτα το Gold επειδή είναι πιο σπάνιο.
            if (roll <= goldChance &&
                goldOrePrefabs != null &&
                goldOrePrefabs.Length > 0)
            {
                SpawnOre(
                    goldOrePrefabs,
                    hit.point,
                    ResourceKind.GoldOre,
                    8,
                    Random.Range(1, 3),
                    $"GoldOre_{goldCount:0000}"
                );

                goldCount++;
            }
            else if (roll <= goldChance + ironChance &&
                     ironOrePrefabs != null &&
                     ironOrePrefabs.Length > 0)
            {
                SpawnOre(
                    ironOrePrefabs,
                    hit.point,
                    ResourceKind.IronOre,
                    6,
                    Random.Range(2, 5),
                    $"IronOre_{ironCount:0000}"
                );

                ironCount++;
            }
        }

        Debug.Log(
            $"OreSpawner generated {ironCount} iron ores and " +
            $"{goldCount} gold ores."
        );
    }

    private void SpawnOre(
        GameObject[] prefabs,
        Vector3 position,
        ResourceKind resourceKind,
        int health,
        int dropAmount,
        string objectName)
    {
        GameObject prefab =
            prefabs[Random.Range(0, prefabs.Length)];

        Quaternion rotation = Quaternion.Euler(
            Random.Range(-6f, 6f),
            Random.Range(0f, 360f),
            Random.Range(-6f, 6f)
        );

        GameObject ore = Instantiate(
            prefab,
            position,
            rotation,
            oresParent
        );

        ore.name = objectName;

        float randomScale =
            Random.Range(minimumScale, maximumScale);

        ore.transform.localScale *= randomScale;

        ResourceNode node =
            ore.GetComponent<ResourceNode>();

        if (node == null)
        {
            node = ore.AddComponent<ResourceNode>();
        }

        node.Initialize(
            resourceKind,
            health,
            dropAmount
        );
    }

    private bool HasAnyPrefabs()
    {
        bool hasIron =
            ironOrePrefabs != null &&
            ironOrePrefabs.Length > 0;

        bool hasGold =
            goldOrePrefabs != null &&
            goldOrePrefabs.Length > 0;

        return hasIron || hasGold;
    }

    private void ClearExistingOres()
    {
        for (int i = oresParent.childCount - 1; i >= 0; i--)
        {
            GameObject child =
                oresParent.GetChild(i).gameObject;

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