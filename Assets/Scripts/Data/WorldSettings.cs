using UnityEngine;

[CreateAssetMenu(
    fileName = "World Settings",
    menuName = "Survival Game/World Settings"
)]
public class WorldSettings : ScriptableObject
{
    [Header("Seed")]
    public int seed = 12345;

    [Header("Chunk Settings")]
    public int chunkSize = 64;
    public int chunksX = 4;
    public int chunksZ = 4;

    [Header("Terrain Settings")]
    public float noiseScale = 25f;
    public float heightMultiplier = 12f;
    public float vertexSpacing = 1f;

    [Header("Island Settings")]
    [Range(0f, 2f)]
    public float islandFalloffStrength = 1f;

    [Range(0.5f, 8f)]
    public float islandFalloffPower = 2.5f;
}