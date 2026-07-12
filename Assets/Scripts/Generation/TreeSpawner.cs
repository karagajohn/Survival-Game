using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [Header("References")]
    public WorldSettings settings;
    public GameObject[] treePrefabs;
    public Transform treesParent;

    [Header("Spawn Settings")]
    [Min(0)]
    public int spawnAttempts = 1200;

    [Range(0f, 1f)]
    public float grassSpawnChance = 0.12f;

    [Range(0f, 1f)]
    public float forestSpawnChance = 0.35f;

    [Header("Placement")]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 28f;

    public float minimumScale = 0.8f;
    public float maximumScale = 1.25f;

    public void GenerateTrees()
    {
        if (settings == null)
        {
            Debug.LogError("TreeSpawner: WorldSettings is missing.");
            return;
        }

        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogError("TreeSpawner: No tree prefabs assigned.");
            return;
        }

        if (treesParent == null)
        {
            treesParent = transform;
        }

        ClearExistingTrees();

        Random.InitState(settings.seed + 10000);

        float worldWidth =
            settings.chunkSize *
            settings.chunksX *
            settings.vertexSpacing;

        float worldDepth =
            settings.chunkSize *
            settings.chunksZ *
            settings.vertexSpacing;

        int spawnedTrees = 0;

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

            // Βεβαιωνόμαστε ότι χτυπήσαμε procedural terrain.
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

            if (biome == BiomeType.Forest)
            {
                spawnChance = forestSpawnChance;
            }
            else if (biome == BiomeType.Grass)
            {
                spawnChance = grassSpawnChance;
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

            SpawnTree(hit.point, spawnedTrees);
            spawnedTrees++;
        }

        Debug.Log($"TreeSpawner generated {spawnedTrees} trees.");
    }

    private void SpawnTree(Vector3 position, int index)
    {
        GameObject prefab =
            treePrefabs[Random.Range(0, treePrefabs.Length)];

        Quaternion rotation = Quaternion.Euler(
            0f,
            Random.Range(0f, 360f),
            0f
        );

        GameObject tree = Instantiate(
            prefab,
            position,
            rotation,
            treesParent
        );

        tree.name = $"Tree_{index:0000}";

        float randomScale =
            Random.Range(minimumScale, maximumScale);

        tree.transform.localScale *= randomScale;
        ResourceNode node = tree.GetComponent<ResourceNode>();

        if (node == null)
        {
            node = tree.AddComponent<ResourceNode>();
        }

        node.Initialize(
            ResourceKind.Wood,
            ResourceNodeType.Tree,
            5,
            Random.Range(3, 7)
        );
    }

    private void ClearExistingTrees()
    {
        for (int i = treesParent.childCount - 1; i >= 0; i--)
        {
            GameObject child =
                treesParent.GetChild(i).gameObject;

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