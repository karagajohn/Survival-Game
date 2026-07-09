using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    [Header("Terrain Size")]
    public int width = 50;
    public int height = 50;

    [Header("Vertices")]
    public float vertexSpacing = 1f;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "Procedural Terrain Mesh";

        GetComponent<MeshFilter>().mesh = mesh;

        CreateVertices();
        CreateTriangles();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    void CreateVertices()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];

        int index = 0;

        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                vertices[index] = new Vector3(x * vertexSpacing, 0, z * vertexSpacing);
                index++;
            }
        }
    }

    void CreateTriangles()
    {
        triangles = new int[width * height * 6];

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + width + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + width + 1;
                triangles[triangleIndex + 5] = vertexIndex + width + 2;

                vertexIndex++;
                triangleIndex += 6;
            }

            vertexIndex++;
        }
    }
}