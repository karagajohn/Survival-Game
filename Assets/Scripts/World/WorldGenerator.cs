using UnityEngine;

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
    }

    private void GenerateChunks()
    {
        for (int x = 0; x < settings.chunksX; x++)
        {
            for (int z = 0; z < settings.chunksZ; z++)
            {
                GameObject chunkObject = new GameObject($"Chunk_{x}_{z}");
                chunkObject.transform.parent = terrainParent;

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