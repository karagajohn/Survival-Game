using UnityEngine;

[CreateAssetMenu(
    fileName = "World Settings",
    menuName = "Survival Game/World Settings")]
public class WorldSettings : ScriptableObject
{
    [Header("Seed")]
    public int seed = 12345;

    [Header("Chunk")]

    public int chunkSize = 64;

    public int chunksX = 4;

    public int chunksZ = 4;

    [Header("Terrain")]

    public float noiseScale = 25;

    public float heightMultiplier = 12;

    public float vertexSpacing = 1;
}