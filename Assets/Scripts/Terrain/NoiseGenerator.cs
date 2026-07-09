using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(
        int width,
        int height,
        float scale,
        int seed)
    {
        float[,] noiseMap = new float[width, height];

        System.Random random = new System.Random(seed);

        float offsetX = random.Next(-100000, 100000);
        float offsetY = random.Next(-100000, 100000);

        if (scale <= 0)
            scale = 0.0001f;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = (x + offsetX) / scale;
                float sampleY = (z + offsetY) / scale;

                float noise = Mathf.PerlinNoise(sampleX, sampleY);

                noiseMap[x, z] = noise;
            }
        }

        return noiseMap;
    }
}