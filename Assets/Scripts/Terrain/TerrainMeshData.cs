using UnityEngine;

public class TerrainMeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Color[] colors;

    public TerrainMeshData(int vertexCount, int triangleCount)
    {
        vertices = new Vector3[vertexCount];
        triangles = new int[triangleCount];
        uvs = new Vector2[vertexCount];
        colors = new Color[vertexCount];
    }
}