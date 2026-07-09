using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainChunk : MonoBehaviour
{
    private TerrainGenerator terrainGenerator;

    public void Initialize(WorldSettings settings, int chunkX, int chunkZ)
    {
        gameObject.name = $"Chunk_{chunkX}_{chunkZ}";

        transform.position = new Vector3(
            chunkX * settings.chunkSize * settings.vertexSpacing,
            0,
            chunkZ * settings.chunkSize * settings.vertexSpacing
        );

        terrainGenerator = gameObject.AddComponent<TerrainGenerator>();

        terrainGenerator.width = settings.chunkSize;
        terrainGenerator.height = settings.chunkSize;
        terrainGenerator.vertexSpacing = settings.vertexSpacing;
        terrainGenerator.seed = settings.seed + chunkX * 1000 + chunkZ;
        terrainGenerator.noiseScale = settings.noiseScale;
        terrainGenerator.heightMultiplier = settings.heightMultiplier;

        terrainGenerator.GenerateTerrain();
    }
}