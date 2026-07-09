using UnityEngine;

[System.Serializable]
public class OldWorldSettings
{
    [Header("Seed")]
    public int seed = 12345;

    [Header("World Size")]
    public int chunkSize = 64;
    public int chunksX = 4;
    public int chunksZ = 4;

    [Header("Terrain")]
    public float noiseScale = 25f;
    public float heightMultiplier = 12f;
}