using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;

    public float maxHunger = 100f;
    public float hunger = 100f;

    public float maxStamina = 100f;
    public float stamina = 100f;

    public float hungerDrainPerSecond = 0.5f;
    public float healthDrainWhenStarving = 5f;
    public float staminaRegenPerSecond = 15f;

    public event Action OnChanged;

    private void Update()
    {
        hunger -= hungerDrainPerSecond * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);

        if (hunger <= 0f)
        {
            Damage(healthDrainWhenStarving * Time.deltaTime);
        }

        stamina += staminaRegenPerSecond * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);

        OnChanged?.Invoke();
    }

    public void Damage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0f, maxHealth);

        if (health <= 0f)
        {
            Debug.Log("Player died!");
            // Εδώ αργότερα βάζουμε Game Over screen.
        }

        OnChanged?.Invoke();
    }

    public void Eat(float amount)
    {
        hunger += amount;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        OnChanged?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (stamina < amount)
            return false;

        stamina -= amount;
        OnChanged?.Invoke();
        return true;
    }
}