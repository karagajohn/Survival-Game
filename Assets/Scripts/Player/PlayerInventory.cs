using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public int wood;
    public int stone;
    public int food;

    public event Action OnChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Add(ResourceKind kind, int amount)
    {
        switch (kind)
        {
            case ResourceKind.Wood:
                wood += amount;
                break;

            case ResourceKind.Stone:
                stone += amount;
                break;

            case ResourceKind.Food:
                food += amount;
                break;
        }

        Debug.Log($"Added {amount} {kind}");
        OnChanged?.Invoke();
    }

    public bool Has(ResourceKind kind, int amount)
    {
        return Get(kind) >= amount;
    }

    public bool Spend(ResourceKind kind, int amount)
    {
        if (!Has(kind, amount))
            return false;

        switch (kind)
        {
            case ResourceKind.Wood:
                wood -= amount;
                break;

            case ResourceKind.Stone:
                stone -= amount;
                break;

            case ResourceKind.Food:
                food -= amount;
                break;
        }

        OnChanged?.Invoke();
        return true;
    }

    public int Get(ResourceKind kind)
    {
        switch (kind)
        {
            case ResourceKind.Wood:
                return wood;

            case ResourceKind.Stone:
                return stone;

            case ResourceKind.Food:
                return food;

            default:
                return 0;
        }
    }
}