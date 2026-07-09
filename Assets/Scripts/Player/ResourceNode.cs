using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceKind resourceKind = ResourceKind.Wood;
    public int maxHealth = 3;
    public int dropAmount = 3;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Hit(int damage)
    {
        currentHealth -= damage;

        Debug.Log($"{gameObject.name} hit. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Harvest();
        }
    }

    private void Harvest()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.Add(resourceKind, dropAmount);
        }

        Destroy(gameObject);
    }
}