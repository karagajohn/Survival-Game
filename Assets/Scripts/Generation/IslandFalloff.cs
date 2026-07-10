using UnityEngine;

public static class IslandFalloff
{
    public static float Evaluate(
    float globalX,
    float globalZ,
    float worldWidth,
    float worldDepth,
    float power)
{
    float centerX = worldWidth * 0.5f;
    float centerZ = worldDepth * 0.5f;

    float dx = (globalX - centerX) / centerX;
    float dz = (globalZ - centerZ) / centerZ;

    float distance = Mathf.Sqrt(dx * dx + dz * dz);

    return Mathf.Pow(Mathf.Clamp01(distance), power);
}
}