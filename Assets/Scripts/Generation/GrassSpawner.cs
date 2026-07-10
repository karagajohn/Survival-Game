using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GrassSpawner : MonoBehaviour
{
    [Header("References")]
    public WorldSettings settings;
    public GameObject grassPrefab;
    public Material grassMaterial;

    [Header("Generation")]
    [Min(0)]
    public int spawnAttempts = 6000;

    [Range(0f, 1f)]
    public float grassBiomeChance = 0.55f;

    [Range(0f, 1f)]
    public float forestBiomeChance = 0.30f;

    [Header("Placement")]
    public float raycastHeight = 100f;

    [Range(0f, 60f)]
    public float maximumSlope = 32f;

    public float minimumScale = 0.65f;
    public float maximumScale = 1.20f;

    [Tooltip("Μικρή ανύψωση για να μην μπαίνει το grass μέσα στο terrain.")]
    public float verticalOffset = 0.02f;

    [Header("Rendering")]
    public bool castShadows = false;
    public bool receiveShadows = true;

    private const int MaxInstancesPerBatch = 1023;

    private Mesh grassMesh;
    private readonly List<Matrix4x4[]> batches = new();
    private readonly List<int> batchCounts = new();

    private bool isReady;

    public void GenerateGrass()
    {
        if (!ValidateReferences())
        {
            return;
        }

        ExtractPrefabData();

        if (grassMesh == null || grassMaterial == null)
        {
            return;
        }

        grassMaterial.enableInstancing = true;

        batches.Clear();
        batchCounts.Clear();

        Random.InitState(settings.seed + 50000);

        float worldWidth =
            settings.chunkSize *
            settings.chunksX *
            settings.vertexSpacing;

        float worldDepth =
            settings.chunkSize *
            settings.chunksZ *
            settings.vertexSpacing;

        List<Matrix4x4> matrices = new();

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

            // Το grass πρέπει να τοποθετείται μόνο στο terrain.
            if (hit.collider.GetComponent<TerrainChunk>() == null)
            {
                continue;
            }

            float normalizedHeight = Mathf.Clamp01(
                hit.point.y / settings.heightMultiplier
            );

            BiomeType biome =
                BiomeGenerator.GetBiome(normalizedHeight);

            float spawnChance;

            if (biome == BiomeType.Grass)
            {
                spawnChance = grassBiomeChance;
            }
            else if (biome == BiomeType.Forest)
            {
                spawnChance = forestBiomeChance;
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

            Vector3 position =
                hit.point + Vector3.up * verticalOffset;

            Quaternion rotation = Quaternion.Euler(
                0f,
                Random.Range(0f, 360f),
                0f
            );

            float scale =
                Random.Range(minimumScale, maximumScale);

            Vector3 scaleVector =
                Vector3.one * scale;

            Matrix4x4 matrix = Matrix4x4.TRS(
                position,
                rotation,
                scaleVector
            );

            matrices.Add(matrix);
        }

        CreateBatches(matrices);

        isReady = batches.Count > 0;

        Debug.Log(
            $"GrassSpawner generated {matrices.Count} grass instances " +
            $"in {batches.Count} batches."
        );
    }

    private void Update()
    {
        if (!isReady ||
            grassMesh == null ||
            grassMaterial == null)
        {
            return;
        }

        ShadowCastingMode shadowMode =
            castShadows
                ? ShadowCastingMode.On
                : ShadowCastingMode.Off;

        for (int i = 0; i < batches.Count; i++)
        {
            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                batches[i],
                batchCounts[i],
                null,
                shadowMode,
                receiveShadows,
                gameObject.layer
            );
        }
    }

    private bool ValidateReferences()
    {
        if (settings == null)
        {
            Debug.LogError(
                "GrassSpawner: WorldSettings is missing."
            );

            return false;
        }

        if (grassPrefab == null)
        {
            Debug.LogError(
                "GrassSpawner: Grass prefab is missing."
            );

            return false;
        }

        return true;
    }

    private void ExtractPrefabData()
    {
        MeshFilter meshFilter =
            grassPrefab.GetComponentInChildren<MeshFilter>();

        MeshRenderer meshRenderer =
            grassPrefab.GetComponentInChildren<MeshRenderer>();

        if (meshFilter == null ||
            meshFilter.sharedMesh == null)
        {
            Debug.LogError(
                "GrassSpawner: The prefab has no usable MeshFilter."
            );

            return;
        }

        grassMesh = meshFilter.sharedMesh;

        if (grassMaterial == null &&
            meshRenderer != null)
        {
            grassMaterial = meshRenderer.sharedMaterial;
        }

        if (grassMaterial == null)
        {
            Debug.LogError(
                "GrassSpawner: No grass material is assigned."
            );
        }
    }

    private void CreateBatches(List<Matrix4x4> matrices)
    {
        int startIndex = 0;

        while (startIndex < matrices.Count)
        {
            int count = Mathf.Min(
                MaxInstancesPerBatch,
                matrices.Count - startIndex
            );

            Matrix4x4[] batch =
                new Matrix4x4[count];

            matrices.CopyTo(
                startIndex,
                batch,
                0,
                count
            );

            batches.Add(batch);
            batchCounts.Add(count);

            startIndex += count;
        }
    }
}