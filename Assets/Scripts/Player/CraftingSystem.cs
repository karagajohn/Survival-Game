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

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EatFood();
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