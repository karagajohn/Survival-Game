using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int width, int height)
    {
        float[,] map = new float[width, height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = x / (float)width * 2f - 1f;
                float nz = z / (float)height * 2f - 1f;

                float value = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(nz));

                map[x, z] = Evaluate(value);
            }
        }

        return map;
    }

    private static float Evaluate(float value)
    {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(value, a) / 
               (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}