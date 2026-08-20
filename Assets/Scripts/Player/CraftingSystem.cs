using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public PlayerInteraction interaction;
    public PlayerStats stats;

    private void Start()
    {
        if (interaction == null)
        {
            interaction =
                GetComponent<PlayerInteraction>();
        }

        if (stats == null)
        {
            stats =
                GetComponent<PlayerStats>();
        }
    }
}