using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private PlayerInventory inventory;

    [Header("UI References")]
    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private Transform slotsContainer;

    [SerializeField]
    private InventorySlotUI slotPrefab;

    [Header("Player References")]
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private PlayerInteraction playerInteraction;

    private readonly List<InventorySlotUI> slotViews = new();

    public bool IsOpen { get; private set; }

    private void Start()
    {
        FindReferences();

        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
            inventory.OnChanged += Refresh;
        }

        CreateSlotViews();
        SetInventoryOpen(false);
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetInventoryOpen(!IsOpen);
        }
    }

    private void FindReferences()
    {
        if (inventory == null)
        {
            inventory = PlayerInventory.Instance;
        }

        if (playerController == null)
        {
            playerController =
                FindAnyObjectByType<PlayerController>();
        }

        if (playerInteraction == null)
        {
            playerInteraction =
                FindAnyObjectByType<PlayerInteraction>();
        }
    }

    private void CreateSlotViews()
    {
        if (inventory == null ||
            slotsContainer == null ||
            slotPrefab == null)
        {
            Debug.LogError(
                "InventoryUI: Missing required references."
            );

            return;
        }

        for (int i = slotsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(
                slotsContainer.GetChild(i).gameObject
            );
        }

        slotViews.Clear();

        for (int i = 0; i < inventory.Capacity; i++)
        {
            InventorySlotUI slotView = Instantiate(
                slotPrefab,
                slotsContainer
            );

            slotView.name = $"InventorySlot_{i:00}";
            slotViews.Add(slotView);
        }
    }

    public void Refresh()
    {
        if (inventory == null)
        {
            return;
        }

        IReadOnlyList<InventorySlot> slots =
            inventory.Slots;

        for (int i = 0; i < slotViews.Count; i++)
        {
            if (i < slots.Count)
            {
                slotViews[i].Display(slots[i]);
            }
            else
            {
                slotViews[i].Clear();
            }
        }
    }

    public void SetInventoryOpen(bool open)
    {
        IsOpen = open;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(open);
        }

        if (playerController != null)
        {
            playerController.SetInputEnabled(!open);
        }

        if (playerInteraction != null)
        {
            playerInteraction.enabled = !open;
        }

        Cursor.lockState = open
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = open;
    }
}