using UnityEngine;

public enum BiomeType
{
    Water,
    Sand,
    Grass,
    Forest,
    Rock,
    Snow
}

public static class BiomeGenerator
{
    public static BiomeType GetBiome(float height)
    {
        if (height < 0.18f)
            return BiomeType.Water;

        if (height < 0.28f)
            return BiomeType.Sand;

        if (height < 0.55f)
            return BiomeType.Grass;

        if (height < 0.75f)
            return BiomeType.Forest;

        if (height < 0.90f)
            return BiomeType.Rock;

        return BiomeType.Snow;
    }

    public static Color GetColor(float height)
    {
        BiomeType biome = GetBiome(height);

        switch (biome)
        {
            case BiomeType.Water:
                return Color.blue;

            case BiomeType.Sand:
                return Color.yellow;

            case BiomeType.Grass:
                return Color.green;

            case BiomeType.Forest:
                return new Color(0f, 0.35f, 0f);

            case BiomeType.Rock:
                return Color.gray;

            case BiomeType.Snow:
                return Color.white;

            default:
                return Color.magenta;
        }
    }
}