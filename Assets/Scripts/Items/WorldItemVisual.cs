using UnityEngine;

public class WorldItemVisual : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 60f;

    [Header("Floating")]
    public float floatHeight = 0.15f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );

        float verticalOffset =
            Mathf.Sin(Time.time * floatSpeed) *
            floatHeight;

        transform.localPosition =
            startPosition +
            Vector3.up * verticalOffset;
    }
}