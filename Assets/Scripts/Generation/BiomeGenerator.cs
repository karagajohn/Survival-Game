using UnityEngine;

public enum BiomeType
{
    Water,
    Sand,
    Grass,
    Forest,
    Rock
}

public static class BiomeGenerator
{
    public static BiomeType GetBiome(float height)
    {
        if (height < 0.18f)
            return BiomeType.Water;

        if (height < 0.28f)
            return BiomeType.Sand;

        if (height < 0.58f)
            return BiomeType.Grass;

        if (height < 0.74f)
            return BiomeType.Forest;

        return BiomeType.Rock;
    }

    public static Color GetColor(float height)
    {
        switch (GetBiome(height))
        {
            case BiomeType.Water:
                return new Color(0.05f, 0.25f, 0.85f);

            case BiomeType.Sand:
                return new Color(0.90f, 0.80f, 0.35f);

            case BiomeType.Grass:
                return new Color(0.18f, 0.72f, 0.12f);

            case BiomeType.Forest:
                return new Color(0.03f, 0.32f, 0.06f);

            case BiomeType.Rock:
                return new Color(0.15f, 0.16f, 0.18f);

            default:
                return Color.magenta;
        }
    }
}