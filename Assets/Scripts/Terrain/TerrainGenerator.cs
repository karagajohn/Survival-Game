using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour
{
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
            seed
        );

        float[,] falloffMap = FalloffGenerator.GenerateFalloffMap(
            width + 1,
            height + 1
        );

        TerrainMeshData meshData = CreateTerrainMesh(heightMap, falloffMap);

        mesh = new Mesh();
        mesh.name = "Procedural Island Terrain";

        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;
        mesh.uv = meshData.uvs;

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private TerrainMeshData CreateTerrainMesh(float[,] heightMap, float[,] falloffMap)
    {
        int vertexCount = (width + 1) * (height + 1);
        int triangleCount = width * height * 6;

        TerrainMeshData meshData = new TerrainMeshData(vertexCount, triangleCount);

        int vertexIndex = 0;

        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float heightValue = Mathf.Clamp01(heightMap[x, z] - falloffMap[x, z]);
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

                vertexIndex++;
            }
        }

        int triangleIndex = 0;
        int currentVertex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.triangles[triangleIndex + 0] = currentVertex;
                meshData.triangles[triangleIndex + 1] = currentVertex + width + 1;
                meshData.triangles[triangleIndex + 2] = currentVertex + 1;

                meshData.triangles[triangleIndex + 3] = currentVertex + 1;
                meshData.triangles[triangleIndex + 4] = currentVertex + width + 1;
                meshData.triangles[triangleIndex + 5] = currentVertex + width + 2;

                currentVertex++;
                triangleIndex += 6;
            }

            currentVertex++;
        }

        return meshData;
    }
}