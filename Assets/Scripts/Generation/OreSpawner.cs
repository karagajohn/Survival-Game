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
    [Min(1f)]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 18f;

    [Min(0.01f)]
    public float minimumScale = 0.70f;

    [Min(0.01f)]
    public float maximumScale = 1.15f;

    [Tooltip("How far the ore is buried into the terrain.")]
    [Min(0f)]
    public float groundSinkDepth = 0.03f;

    public void GenerateOres()
    {
        if (settings == null)
        {
            Debug.LogError(
                "OreSpawner: WorldSettings is missing."
            );

            return;
        }

        if (!HasAnyPrefabs())
        {
            Debug.LogError(
                "OreSpawner: No valid ore prefabs assigned."
            );

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

            bool hitSomething = Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                raycastHeight * 2f,
                ~0,
                QueryTriggerInteraction.Ignore
            );

            if (!hitSomething)
            {
                continue;
            }

            TerrainChunk terrainChunk =
                hit.collider.GetComponentInParent<TerrainChunk>();

            if (terrainChunk == null)
            {
                continue;
            }

            float normalizedHeight = Mathf.Clamp01(
                hit.point.y /
                settings.heightMultiplier
            );

            BiomeType biome =
                BiomeGenerator.GetBiome(normalizedHeight);

            if (biome != BiomeType.Grass &&
                biome != BiomeType.Rock)
            {
                continue;
            }

            float slopeAngle = Vector3.Angle(
                hit.normal,
                Vector3.up
            );

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

            if (roll <= goldChance &&
                HasValidPrefab(goldOrePrefabs))
            {
                if (SpawnOre(
                        goldOrePrefabs,
                        hit.point,
                        hit.normal,
                        ResourceKind.GoldOre,
                        8,
                        Random.Range(1, 3),
                        $"GoldOre_{goldCount:0000}"))
                {
                    goldCount++;
                }
            }
            else if (
                roll <= goldChance + ironChance &&
                HasValidPrefab(ironOrePrefabs))
            {
                if (SpawnOre(
                        ironOrePrefabs,
                        hit.point,
                        hit.normal,
                        ResourceKind.IronOre,
                        6,
                        Random.Range(2, 5),
                        $"IronOre_{ironCount:0000}"))
                {
                    ironCount++;
                }
            }
        }

        Debug.Log(
            $"OreSpawner generated {ironCount} iron ores " +
            $"and {goldCount} gold ores."
        );
    }

    private bool SpawnOre(
        GameObject[] prefabs,
        Vector3 groundPosition,
        Vector3 groundNormal,
        ResourceKind resourceKind,
        int health,
        int dropAmount,
        string objectName
    )
    {
        GameObject prefab =
            GetRandomValidPrefab(prefabs);

        if (prefab == null)
        {
            return false;
        }

        groundNormal.Normalize();

        Quaternion alignToGround =
            Quaternion.FromToRotation(
                Vector3.up,
                groundNormal
            );

        Quaternion randomYaw =
            Quaternion.AngleAxis(
                Random.Range(0f, 360f),
                groundNormal
            );

        Quaternion rotation =
            randomYaw * alignToGround;

        GameObject ore = Instantiate(
            prefab,
            groundPosition,
            rotation,
            oresParent
        );

        ore.name = objectName;

        float safeMinimumScale =
            Mathf.Max(0.01f, minimumScale);

        float safeMaximumScale =
            Mathf.Max(
                safeMinimumScale,
                maximumScale
            );

        float randomScale = Random.Range(
            safeMinimumScale,
            safeMaximumScale
        );

        ore.transform.localScale *= randomScale;

        SnapObjectToGround(
            ore,
            groundPosition,
            groundNormal
        );

        ResourceNode node =
            ore.GetComponent<ResourceNode>();

        if (node == null)
        {
            node = ore.AddComponent<ResourceNode>();
        }

        node.Initialize(
            resourceKind,
            ResourceNodeType.Ore,
            health,
            dropAmount
        );

        return true;
    }

    private void SnapObjectToGround(
        GameObject spawnedObject,
        Vector3 groundPosition,
        Vector3 groundNormal
    )
    {
        if (!TryGetLowestVertexProjection(
                spawnedObject,
                groundNormal,
                out float lowestProjection))
        {
            Debug.LogWarning(
                $"{spawnedObject.name}: No MeshFilter " +
                "with a valid mesh was found."
            );

            return;
        }

        float groundProjection =
            Vector3.Dot(
                groundPosition,
                groundNormal
            );

        float targetProjection =
            groundProjection - groundSinkDepth;

        float movementDistance =
            targetProjection - lowestProjection;

        spawnedObject.transform.position +=
            groundNormal * movementDistance;
    }

    private bool TryGetLowestVertexProjection(
        GameObject targetObject,
        Vector3 groundNormal,
        out float lowestProjection
    )
    {
        lowestProjection =
            float.PositiveInfinity;

        bool foundVertex = false;

        MeshFilter[] meshFilters =
            targetObject.GetComponentsInChildren<MeshFilter>(
                true
            );

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null ||
                meshFilter.sharedMesh == null)
            {
                continue;
            }

            Vector3[] vertices =
                meshFilter.sharedMesh.vertices;

            Transform meshTransform =
                meshFilter.transform;

            foreach (Vector3 localVertex in vertices)
            {
                Vector3 worldVertex =
                    meshTransform.TransformPoint(
                        localVertex
                    );

                float projection =
                    Vector3.Dot(
                        worldVertex,
                        groundNormal
                    );

                if (projection < lowestProjection)
                {
                    lowestProjection = projection;
                }

                foundVertex = true;
            }
        }

        return foundVertex;
    }

    private bool HasAnyPrefabs()
    {
        return
            HasValidPrefab(ironOrePrefabs) ||
            HasValidPrefab(goldOrePrefabs);
    }

    private bool HasValidPrefab(
        GameObject[] prefabs
    )
    {
        if (prefabs == null ||
            prefabs.Length == 0)
        {
            return false;
        }

        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null)
            {
                return true;
            }
        }

        return false;
    }

    private GameObject GetRandomValidPrefab(
        GameObject[] prefabs
    )
    {
        if (!HasValidPrefab(prefabs))
        {
            return null;
        }

        int startingIndex =
            Random.Range(0, prefabs.Length);

        for (
            int offset = 0;
            offset < prefabs.Length;
            offset++
        )
        {
            int index =
                (startingIndex + offset) %
                prefabs.Length;

            if (prefabs[index] != null)
            {
                return prefabs[index];
            }
        }

        return null;
    }

    private void ClearExistingOres()
    {
        if (oresParent == null)
        {
            return;
        }

        for (
            int i = oresParent.childCount - 1;
            i >= 0;
            i--
        )
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