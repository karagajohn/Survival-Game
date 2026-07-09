using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 10;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        Debug.Log($"{gameObject.name} took damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}