using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;

    public float dayLengthSeconds = 180f;

    [Range(0f, 1f)]
    public float timeOfDay = 0.25f;

    public bool IsNight
    {
        get
        {
            return timeOfDay > 0.75f || timeOfDay < 0.20f;
        }
    }

    private void Update()
    {
        timeOfDay += Time.deltaTime / dayLengthSeconds;

        if (timeOfDay >= 1f)
            timeOfDay = 0f;

        if (sun != null)
        {
            sun.transform.rotation = Quaternion.Euler(timeOfDay * 360f - 90f, 170f, 0f);
        }
    }
}