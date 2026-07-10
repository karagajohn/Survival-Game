using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public WorldSettings settings;
    public TreeSpawner treeSpawner;
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
        
        if (treeSpawner != null)
        {
            treeSpawner.GenerateTrees();
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