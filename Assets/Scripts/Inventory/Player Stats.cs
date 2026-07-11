using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Hunger")]
    public float maxHunger = 100f;
    public float hunger = 100f;
    public float hungerDrainPerSecond = 0.5f;
    public float healthDrainWhenStarving = 5f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina = 100f;
    public float staminaRegenPerSecond = 15f;
    public float staminaRegenDelay = 0.75f;

    [Header("UI Updates")]
    [Min(0.02f)]
    public float updateInterval = 0.1f;

    public bool IsDead { get; private set; }

    public event Action OnChanged;
    public event Action OnDied;
    public event Action OnRespawned;

    private float lastStaminaUseTime;
    private float nextUpdateTime;

    private void Awake()
    {
        health = Mathf.Clamp(health, 0f, maxHealth);
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }

        bool changed = false;

        changed |= DrainHunger();
        changed |= RegenerateStamina();

        if (changed && Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            NotifyChanged();
        }
    }

    private bool DrainHunger()
    {
        float oldHunger = hunger;

        hunger -= hungerDrainPerSecond * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);

        if (hunger <= 0f)
        {
            DamageInternal(
                healthDrainWhenStarving * Time.deltaTime,
                false
            );
        }

        return !Mathf.Approximately(oldHunger, hunger);
    }

    private bool RegenerateStamina()
    {
        if (Time.time < lastStaminaUseTime + staminaRegenDelay)
        {
            return false;
        }

        if (stamina >= maxStamina)
        {
            return false;
        }

        float oldStamina = stamina;

        stamina += staminaRegenPerSecond * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);

        return !Mathf.Approximately(oldStamina, stamina);
    }

    public void Damage(float amount)
    {
        DamageInternal(amount, true);
    }

    private void DamageInternal(float amount, bool notifyImmediately)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        health -= amount;
        health = Mathf.Clamp(health, 0f, maxHealth);

        if (health <= 0f)
        {
            Die();
            return;
        }

        if (notifyImmediately)
        {
            NotifyChanged();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        health = Mathf.Clamp(
            health + amount,
            0f,
            maxHealth
        );

        NotifyChanged();
    }

    public void Eat(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        hunger = Mathf.Clamp(
            hunger + amount,
            0f,
            maxHunger
        );

        NotifyChanged();
    }

    public bool UseStamina(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return false;
        }

        if (stamina < amount)
        {
            return false;
        }

        stamina -= amount;
        lastStaminaUseTime = Time.time;

        NotifyChanged();
        return true;
    }

    private void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        health = 0f;

        Debug.Log("Player died.");

        NotifyChanged();
        OnDied?.Invoke();
    }

    public void Respawn(float hungerPercentage = 0.7f)
    {
        IsDead = false;

        health = maxHealth;
        stamina = maxStamina;

        hunger = Mathf.Clamp(
            maxHunger * hungerPercentage,
            0f,
            maxHunger
        );

        lastStaminaUseTime = Time.time;

        NotifyChanged();
        OnRespawned?.Invoke();

        Debug.Log("Player respawned.");
    }

    private void NotifyChanged()
    {
        OnChanged?.Invoke();
    }
}