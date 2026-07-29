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
    [Min(1f)]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 35f;

    [Min(0.01f)]
    public float minimumScale = 0.65f;

    [Min(0.01f)]
    public float maximumScale = 1.15f;

    [Tooltip(
        "Percentage of the rock's height that is buried in the terrain."
    )]
    [Range(0f, 0.5f)]
    public float groundSinkFraction = 0.22f;

    [Tooltip(
        "Additional fixed depth used to prevent a visible gap."
    )]
    [Min(0f)]
    public float groundSinkDepth = 0.02f;

    public void GenerateRocks()
    {
        if (settings == null)
        {
            Debug.LogError(
                "RockSpawner: WorldSettings is missing."
            );

            return;
        }

        if (!HasValidPrefab())
        {
            Debug.LogError(
                "RockSpawner: No valid rock prefabs assigned."
            );

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
            float randomX = Random.Range(
                0f,
                worldWidth
            );

            float randomZ = Random.Range(
                0f,
                worldDepth
            );

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
                BiomeGenerator.GetBiome(
                    normalizedHeight
                );

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
                    spawnChance =
                        rockBiomeSpawnChance;
                    break;

                default:
                    continue;
            }

            if (Random.value > spawnChance)
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

            bool spawned = SpawnRock(
                hit.point,
                spawnedRocks
            );

            if (spawned)
            {
                spawnedRocks++;
            }
        }

        Debug.Log(
            $"RockSpawner generated {spawnedRocks} rocks."
        );
    }

    private bool SpawnRock(
        Vector3 groundPosition,
        int index
    )
    {
        GameObject prefab = GetRandomValidPrefab();

        if (prefab == null)
        {
            return false;
        }

        // Only rotate around the Y axis.
        // X/Z rotation could make the rock balance on one vertex.
        Quaternion rotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f
        );

        GameObject rock = Instantiate(
            prefab,
            groundPosition,
            rotation,
            rocksParent
        );

        rock.name = $"Rock_{index:0000}";

        float safeMinimumScale = Mathf.Max(
            0.01f,
            minimumScale
        );

        float safeMaximumScale = Mathf.Max(
            safeMinimumScale,
            maximumScale
        );

        float randomScale = Random.Range(
            safeMinimumScale,
            safeMaximumScale
        );

        rock.transform.localScale *= randomScale;

        SnapObjectToGround(
            rock,
            groundPosition.y
        );

        ResourceNode node =
            rock.GetComponent<ResourceNode>();

        if (node == null)
        {
            node = rock.AddComponent<ResourceNode>();
        }

        node.Initialize(
            ResourceKind.Stone,
            ResourceNodeType.Rock,
            4,
            Random.Range(2, 5)
        );

        return true;
    }

    private void SnapObjectToGround(
        GameObject spawnedObject,
        float groundHeight
    )
    {
        if (spawnedObject == null)
        {
            return;
        }

        if (!TryGetObjectBounds(
                spawnedObject,
                out Bounds objectBounds))
        {
            Debug.LogWarning(
                $"{spawnedObject.name}: No Renderer or Collider " +
                "was found for ground alignment."
            );

            return;
        }

        float proportionalSink =
            objectBounds.size.y *
            groundSinkFraction;

        float targetBottomHeight =
            groundHeight -
            proportionalSink -
            groundSinkDepth;

        float verticalOffset =
            targetBottomHeight -
            objectBounds.min.y;

        spawnedObject.transform.position +=
            Vector3.up * verticalOffset;
    }

    private bool TryGetObjectBounds(
        GameObject targetObject,
        out Bounds combinedBounds
    )
    {
        combinedBounds = new Bounds();
        bool foundBounds = false;

        Renderer[] renderers =
            targetObject.GetComponentsInChildren<Renderer>(
                true
            );

        foreach (Renderer objectRenderer in renderers)
        {
            if (objectRenderer == null)
            {
                continue;
            }

            if (!foundBounds)
            {
                combinedBounds =
                    objectRenderer.bounds;

                foundBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(
                    objectRenderer.bounds
                );
            }
        }

        if (foundBounds)
        {
            return true;
        }

        Collider[] colliders =
            targetObject.GetComponentsInChildren<Collider>(
                true
            );

        foreach (Collider objectCollider in colliders)
        {
            if (objectCollider == null)
            {
                continue;
            }

            if (!foundBounds)
            {
                combinedBounds =
                    objectCollider.bounds;

                foundBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(
                    objectCollider.bounds
                );
            }
        }

        return foundBounds;
    }

    private bool HasValidPrefab()
    {
        if (rockPrefabs == null ||
            rockPrefabs.Length == 0)
        {
            return false;
        }

        foreach (GameObject prefab in rockPrefabs)
        {
            if (prefab != null)
            {
                return true;
            }
        }

        return false;
    }

    private GameObject GetRandomValidPrefab()
    {
        if (!HasValidPrefab())
        {
            return null;
        }

        int startingIndex =
            Random.Range(0, rockPrefabs.Length);

        for (
            int offset = 0;
            offset < rockPrefabs.Length;
            offset++
        )
        {
            int index =
                (startingIndex + offset) %
                rockPrefabs.Length;

            if (rockPrefabs[index] != null)
            {
                return rockPrefabs[index];
            }
        }

        return null;
    }

    private void ClearExistingRocks()
    {
        if (rocksParent == null)
        {
            return;
        }

        for (
            int i = rocksParent.childCount - 1;
            i >= 0;
            i--
        )
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