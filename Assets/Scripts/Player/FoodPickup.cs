using UnityEngine;

public class FoodPickup : MonoBehaviour
{
    public int foodAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.Add(ResourceKind.Food, foodAmount);
            Destroy(gameObject);
        }
    }
}