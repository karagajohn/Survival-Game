using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("World Size")]
    public int totalWorldWidth;
    public int totalWorldDepth;

    [Header("Island Settings")]
    public float islandFalloffStrength = 1f;
    public float islandFalloffPower = 2.5f;

    [Header("World Coordinates")]
    public int worldOffsetX;
    public int worldOffsetZ;

    [Header("Terrain Settings")]
    public int width = 64;
    public int height = 64;
    public float vertexSpacing = 1f;

    [Header("Noise Settings")]
    public int seed = 12345;
    public float noiseScale = 25f;
    public float heightMultiplier = 8f;

    private Mesh mesh;

    public void GenerateTerrain()
    {
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(
            width + 1,
            height + 1,
            noiseScale,
            seed,
            worldOffsetX,
            worldOffsetZ
        );

        TerrainMeshData meshData = CreateTerrainMesh(heightMap);

        mesh = new Mesh();
        mesh.name = "Procedural Terrain";

        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;
        mesh.uv = meshData.uvs;
        mesh.colors = meshData.colors;

        Debug.Log(
            $"{gameObject.name}: Colors = {mesh.colors.Length}, " +
            $"First color = {mesh.colors[0]}"
        );

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private TerrainMeshData CreateTerrainMesh(float[,] heightMap)
    {
        int vertexCount = (width + 1) * (height + 1);
        int triangleCount = width * height * 6;

        TerrainMeshData meshData =
            new TerrainMeshData(vertexCount, triangleCount);

        int vertexIndex = 0;

        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float globalX = worldOffsetX + x;
                float globalZ = worldOffsetZ + z;

                float falloff = IslandFalloff.Evaluate(
                    globalX,
                    globalZ,
                    totalWorldWidth,
                    totalWorldDepth,
                    islandFalloffPower
                );

                float heightValue = Mathf.Clamp01(
                    heightMap[x, z] - falloff * islandFalloffStrength
                );

                float y = heightValue * heightMultiplier;

                meshData.vertices[vertexIndex] = new Vector3(
                    x * vertexSpacing,
                    y,
                    z * vertexSpacing
                );

                meshData.uvs[vertexIndex] = new Vector2(
                    x / (float)width,
                    z / (float)height
                );

                meshData.colors[vertexIndex] =
                    BiomeGenerator.GetColor(heightValue);

                vertexIndex++;
            }
        }

        int triangleIndex = 0;
        int currentVertex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.triangles[triangleIndex] = currentVertex;
                meshData.triangles[triangleIndex + 1] =
                    currentVertex + width + 1;
                meshData.triangles[triangleIndex + 2] =
                    currentVertex + 1;

                meshData.triangles[triangleIndex + 3] =
                    currentVertex + 1;
                meshData.triangles[triangleIndex + 4] =
                    currentVertex + width + 1;
                meshData.triangles[triangleIndex + 5] =
                    currentVertex + width + 2;

                currentVertex++;
                triangleIndex += 6;
            }

            currentVertex++;
        }

        return meshData;
    }
}