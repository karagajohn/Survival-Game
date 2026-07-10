using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(
        int width,
        int height,
        float scale,
        int seed,
        int worldOffsetX,
        int worldOffsetZ)
    {
        float[,] noiseMap = new float[width, height];

        System.Random random = new System.Random(seed);

        float seedOffsetX = random.Next(-100000, 100000);
        float seedOffsetZ = random.Next(-100000, 100000);

        if (scale <= 0f)
        {
            scale = 0.0001f;
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float globalX = worldOffsetX + x;
                float globalZ = worldOffsetZ + z;

                float sampleX = (globalX + seedOffsetX) / scale;
                float sampleZ = (globalZ + seedOffsetZ) / scale;

                noiseMap[x, z] = Mathf.PerlinNoise(sampleX, sampleZ);
            }
        }

        return noiseMap;
    }
}