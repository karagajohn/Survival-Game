using UnityEngine;

public class PrototypeHUD : MonoBehaviour
{
    public PlayerStats stats;
    public PlayerInventory inventory;
    public DayNightCycle dayNightCycle;

    private void Start()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 320, 300), GUI.skin.box);

        GUILayout.Label("SURVIVAL PROTOTYPE");

        if (stats != null)
        {
            GUILayout.Label($"Health: {stats.health:0}/{stats.maxHealth}");
            GUILayout.Label($"Hunger: {stats.hunger:0}/{stats.maxHunger}");
            GUILayout.Label($"Stamina: {stats.stamina:0}/{stats.maxStamina}");
        }

        GUILayout.Space(10);

        if (inventory != null)
        {
            GUILayout.Label($"Wood: {inventory.wood}");
            GUILayout.Label($"Stone: {inventory.stone}");
            GUILayout.Label($"Food: {inventory.food}");
            GUILayout.Label($"Iron Ore: {inventory.ironOre}");
            GUILayout.Label($"Gold Ore: {inventory.goldOre}");
        }

        GUILayout.Space(10);

        if (dayNightCycle != null)
        {
            GUILayout.Label(dayNightCycle.IsNight ? "Time: Night" : "Time: Day");
        }

        GUILayout.Space(10);

        GUILayout.Label("Left Click: Hit");
        GUILayout.Label("1: Stone Axe");
        GUILayout.Label("2: Club");
        GUILayout.Label("3: Eat Food");

        GUILayout.EndArea();
    }
}