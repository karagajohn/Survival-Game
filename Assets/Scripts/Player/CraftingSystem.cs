using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public PlayerInteraction interaction;
    public PlayerStats stats;

    private void Start()
    {
        if (interaction == null)
            interaction = GetComponent<PlayerInteraction>();

        if (stats == null)
            stats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CraftStoneAxe();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CraftClub();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EatFood();
        }
    }

    private void CraftStoneAxe()
    {
        PlayerInventory inv = PlayerInventory.Instance;

        if (inv == null)
            return;

        if (inv.Has(ResourceKind.Wood, 5) && inv.Has(ResourceKind.Stone, 2))
        {
            inv.Spend(ResourceKind.Wood, 5);
            inv.Spend(ResourceKind.Stone, 2);

            interaction.UpgradeToStoneAxe();
            Debug.Log("Crafted Stone Axe!");
        }
        else
        {
            Debug.Log("Need 5 Wood and 2 Stone for Stone Axe.");
        }
    }

    private void CraftClub()
    {
        PlayerInventory inv = PlayerInventory.Instance;

        if (inv == null)
            return;

        if (inv.Has(ResourceKind.Wood, 4))
        {
            inv.Spend(ResourceKind.Wood, 4);

            interaction.UpgradeToClub();
            Debug.Log("Crafted Club!");
        }
        else
        {
            Debug.Log("Need 4 Wood for Club.");
        }
    }

    private void EatFood()
    {
        PlayerInventory inv = PlayerInventory.Instance;

        if (inv == null || stats == null)
            return;

        if (inv.Has(ResourceKind.Food, 1))
        {
            inv.Spend(ResourceKind.Food, 1);
            stats.Eat(25f);

            Debug.Log("Ate food. Hunger restored.");
        }
        else
        {
            Debug.Log("No food available.");
        }
    }
}