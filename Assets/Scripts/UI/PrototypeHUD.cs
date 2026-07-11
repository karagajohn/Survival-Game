using UnityEngine;

public class PrototypeHUD : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInventory inventory;
    public DayNightCycle dayNightCycle;

    private void Start()
    {
        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
        }

        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(
            new Rect(20, 20, 320, 340),
            GUI.skin.box
        );

        GUILayout.Label("SURVIVAL PROTOTYPE");

        if (stats != null)
        {
            GUILayout.Label(
                $"Health: {stats.health:0}/{stats.maxHealth}"
            );

            GUILayout.Label(
                $"Hunger: {stats.hunger:0}/{stats.maxHunger}"
            );

            GUILayout.Label(
                $"Stamina: {stats.stamina:0}/{stats.maxStamina}"
            );
        }

        GUILayout.Space(10);

        if (inventory != null &&
            inventory.itemRegistry != null)
        {
            GUILayout.Label(
                $"Wood: {inventory.Get(ResourceKind.Wood)}"
            );

            GUILayout.Label(
                $"Stone: {inventory.Get(ResourceKind.Stone)}"
            );

            GUILayout.Label(
                $"Food: {inventory.Get(ResourceKind.Food)}"
            );

            GUILayout.Label(
                $"Iron Ore: {inventory.Get(ResourceKind.IronOre)}"
            );

            GUILayout.Label(
                $"Gold Ore: {inventory.Get(ResourceKind.GoldOre)}"
            );
        }
        else
        {
            GUILayout.Label("Inventory not configured.");
        }

        GUILayout.Space(10);

        if (dayNightCycle != null)
        {
            GUILayout.Label(
                dayNightCycle.IsNight
                    ? "Time: Night"
                    : "Time: Day"
            );
        }

        GUILayout.Space(10);

        GUILayout.Label("Left Click: Hit");
        GUILayout.Label("E: Interact");

        GUILayout.EndArea();
    }
}