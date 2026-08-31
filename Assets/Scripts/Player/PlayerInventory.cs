using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("References")]
    public ItemRegistry itemRegistry;

    [Header("Inventory")]
    [Min(1)]
    [SerializeField]
    private int capacity = 24;

    [SerializeField]
    private List<InventorySlot> slots = new();

    public IReadOnlyList<InventorySlot> Slots =>
        slots;

    public int Capacity =>
        capacity;

    public event Action OnChanged;

    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        if (slots == null)
        {
            slots =
                new List<InventorySlot>();
        }

        while (slots.Count < capacity)
        {
            slots.Add(
                new InventorySlot()
            );
        }

        if (slots.Count > capacity)
        {
            slots.RemoveRange(
                capacity,
                slots.Count - capacity
            );
        }
    }

    // =========================================================
    // ADD ITEM
    // =========================================================

    public bool Add(
        ItemData item,
        int amount
    )
    {
        if (item == null ||
            amount <= 0)
        {
            return false;
        }

        int remaining = amount;

        // -----------------------------------------------------
        // TRY STACKING
        // -----------------------------------------------------

        foreach (InventorySlot slot in slots)
        {
            if (!slot.CanStack(item))
            {
                continue;
            }

            remaining =
                slot.Add(
                    item,
                    remaining
                );

            if (remaining <= 0)
            {
                OnChanged?.Invoke();
                return true;
            }
        }

        // -----------------------------------------------------
        // TRY EMPTY SLOTS
        // -----------------------------------------------------

        foreach (InventorySlot slot in slots)
        {
            if (!slot.IsEmpty)
            {
                continue;
            }

            remaining =
                slot.Add(
                    item,
                    remaining
                );

            if (remaining <= 0)
            {
                OnChanged?.Invoke();
                return true;
            }
        }

        OnChanged?.Invoke();

        Debug.LogWarning(
            $"Inventory full. Could not add " +
            $"{remaining}x {item.displayName}."
        );

        return false;
    }

    // =========================================================
    // ADD RESOURCE
    // =========================================================

    public void Add(
        ResourceKind kind,
        int amount
    )
    {
        if (itemRegistry == null)
        {
            Debug.LogError(
                "PlayerInventory: ItemRegistry is missing."
            );

            return;
        }

        ItemData item =
            itemRegistry.GetItem(
                kind
            );

        if (item == null)
        {
            return;
        }

        Add(
            item,
            amount
        );
    }

    // =========================================================
    // REMOVE ITEM
    // =========================================================

    public bool Remove(
        ItemData item,
        int amount
    )
    {
        if (item == null ||
            amount <= 0)
        {
            return false;
        }

        if (GetQuantity(item) < amount)
        {
            return false;
        }

        int remaining = amount;

        for (
            int i = slots.Count - 1;
            i >= 0;
            i--
        )
        {
            InventorySlot slot =
                slots[i];

            if (slot.Item != item)
            {
                continue;
            }

            int amountToRemove =
                Mathf.Min(
                    remaining,
                    slot.Quantity
                );

            slot.Remove(
                amountToRemove
            );

            remaining -=
                amountToRemove;

            if (remaining <= 0)
            {
                OnChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    // =========================================================
    // GET QUANTITY
    // =========================================================

    public int GetQuantity(
        ItemData item
    )
    {
        if (item == null)
        {
            return 0;
        }

        int total = 0;

        foreach (
            InventorySlot slot
            in slots
        )
        {
            if (slot.Item == item)
            {
                total +=
                    slot.Quantity;
            }
        }

        return total;
    }

    // =========================================================
    // RESOURCE HELPERS
    // =========================================================

    public int Get(
        ResourceKind kind
    )
    {
        if (itemRegistry == null)
        {
            return 0;
        }

        ItemData item =
            itemRegistry.GetItem(
                kind
            );

        return GetQuantity(
            item
        );
    }

    public bool Has(
        ResourceKind kind,
        int amount
    )
    {
        return Get(kind) >= amount;
    }

    public bool Spend(
        ResourceKind kind,
        int amount
    )
    {
        if (itemRegistry == null)
        {
            return false;
        }

        ItemData item =
            itemRegistry.GetItem(
                kind
            );

        return Remove(
            item,
            amount
        );
    }

    public bool Contains(
        ItemData item,
        int amount
    )
    {
        return GetQuantity(item) >= amount;
    }

    // =========================================================
    // SAVE SYSTEM SUPPORT
    // =========================================================

    public void ClearAll()
    {
        foreach (
            InventorySlot slot
            in slots
        )
        {
            slot.Clear();
        }

        OnChanged?.Invoke();
    }

    public bool SetSlot(
        int index,
        ItemData item,
        int quantity
    )
    {
        if (index < 0 ||
            index >= slots.Count)
        {
            return false;
        }

        slots[index].Set(
            item,
            quantity
        );

        return true;
    }

    public void NotifyChanged()
    {
        OnChanged?.Invoke();
    }
}