using System.Collections;
using UnityEngine;
using Unity.AI.Navigation;

public class WorldGenerator : MonoBehaviour
{
    public WorldSettings settings;

    public TreeSpawner treeSpawner;
    public RockSpawner rockSpawner;

    public BushSpawner bushSpawner;
    public OreSpawner oreSpawner;
    public GrassSpawner grassSpawner;

    public Transform terrainParent;
    public Material terrainMaterial;
    public WaterGenerator waterGenerator;

    [Header("Navigation")]
    public NavMeshSurface navMeshSurface;

    private void Start()
    {
        if (settings == null)
        {
            Debug.LogError("World Settings missing!");
            return;
        }

        GenerateChunks();

        if (waterGenerator != null)
        {
            waterGenerator.GenerateWater();
        }

        if (grassSpawner != null)
        {
            grassSpawner.GenerateGrass();
        }

        if (treeSpawner != null)
        {
            treeSpawner.GenerateTrees();
        }

        if (rockSpawner != null)
        {
            rockSpawner.GenerateRocks();
        }

        if (bushSpawner != null)
        {
            bushSpawner.GenerateBushes();
        }

        if (oreSpawner != null)
        {
            oreSpawner.GenerateOres();
        }

        StartCoroutine(BuildNavMeshAfterWorldGeneration());
    }

    private IEnumerator BuildNavMeshAfterWorldGeneration()
    {
        // Περιμένουμε ώστε να ολοκληρωθεί η δημιουργία
        // των runtime MeshColliders και των υπόλοιπων objects.
        yield return null;
        yield return new WaitForEndOfFrame();

        if (navMeshSurface == null)
        {
            Debug.LogError("NavMesh Surface missing!");
            yield break;
        }

        Debug.Log("Building NavMesh...");

        navMeshSurface.BuildNavMesh();

        Debug.Log("NavMesh build completed!");
    }

    private void GenerateChunks()
    {
        for (int x = 0; x < settings.chunksX; x++)
        {
            for (int z = 0; z < settings.chunksZ; z++)
            {
                GameObject chunkObject = new GameObject($"Chunk_{x}_{z}");

                chunkObject.transform.parent = terrainParent;

                // Το procedural terrain ανήκει στο Ground layer.
                int groundLayer = LayerMask.NameToLayer("Ground");

                if (groundLayer == -1)
                {
                    Debug.LogError(
                        "Ground layer was not found! " +
                        "Please create a layer named 'Ground'."
                    );
                }
                else
                {
                    chunkObject.layer = groundLayer;
                }

                MeshRenderer renderer = chunkObject.AddComponent<MeshRenderer>();
                renderer.material = terrainMaterial;

                chunkObject.AddComponent<MeshFilter>();
                chunkObject.AddComponent<MeshCollider>();

                TerrainChunk chunk = chunkObject.AddComponent<TerrainChunk>();
                chunk.Initialize(settings, x, z);
            }
        }
    }
}