using UnityEngine;

[RequireComponent(
    typeof(MeshFilter),
    typeof(MeshRenderer),
    typeof(MeshCollider)
)]
public class TerrainChunk : MonoBehaviour
{
    private TerrainGenerator terrainGenerator;

    public void Initialize(
        WorldSettings settings,
        int chunkX,
        int chunkZ
    )
    {
        gameObject.name = $"Chunk_{chunkX}_{chunkZ}";

        transform.position = new Vector3(
            chunkX * settings.chunkSize * settings.vertexSpacing,
            0f,
            chunkZ * settings.chunkSize * settings.vertexSpacing
        );

        terrainGenerator = gameObject.AddComponent<TerrainGenerator>();

        terrainGenerator.width = settings.chunkSize;
        terrainGenerator.height = settings.chunkSize;
        terrainGenerator.vertexSpacing = settings.vertexSpacing;

        terrainGenerator.seed = settings.seed;
        terrainGenerator.noiseScale = settings.noiseScale;
        terrainGenerator.heightMultiplier = settings.heightMultiplier;

        terrainGenerator.worldOffsetX =
            chunkX * settings.chunkSize;

        terrainGenerator.worldOffsetZ =
            chunkZ * settings.chunkSize;

        terrainGenerator.totalWorldWidth =
            settings.chunkSize * settings.chunksX;

        terrainGenerator.totalWorldDepth =
            settings.chunkSize * settings.chunksZ;

        terrainGenerator.islandFalloffStrength =
            settings.islandFalloffStrength;

        terrainGenerator.islandFalloffPower =
            settings.islandFalloffPower;

        terrainGenerator.GenerateTerrain();
    }
}