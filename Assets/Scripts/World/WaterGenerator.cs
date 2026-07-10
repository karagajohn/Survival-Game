using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterGenerator : MonoBehaviour
{
    public WorldSettings settings;
    public float waterHeight = 1.5f;
    public Material waterMaterial;

    public void GenerateWater()
    {
        if (settings == null)
        {
            Debug.LogError("WaterGenerator: Missing WorldSettings.");
            return;
        }

        float width = settings.chunkSize * settings.chunksX * settings.vertexSpacing;
        float depth = settings.chunkSize * settings.chunksZ * settings.vertexSpacing;

        Mesh mesh = new Mesh();
        mesh.name = "Procedural Water Mesh";

        Vector3[] vertices =
        {
            new Vector3(0, waterHeight, 0),
            new Vector3(width, waterHeight, 0),
            new Vector3(0, waterHeight, depth),
            new Vector3(width, waterHeight, depth)
        };

        int[] triangles =
        {
            0, 2, 1,
            1, 2, 3
        };

        Vector2[] uvs =
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        MeshFilter filter = GetComponent<MeshFilter>();
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        filter.mesh = mesh;

        if (waterMaterial != null)
            renderer.material = waterMaterial;
    }
}