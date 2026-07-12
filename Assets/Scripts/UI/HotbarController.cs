using System;
using System.Collections.Generic;
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    private const int HotbarSize = 8;

    [Header("References")]
    [SerializeField]
    private PlayerInventory inventory;

    [SerializeField]
    private InventoryUI inventoryUI;

    [SerializeField]
    private Transform slotsContainer;

    [SerializeField]
    private HotbarSlotUI slotPrefab;

    private readonly List<HotbarSlotUI> slotViews = new();

    public int SelectedIndex { get; private set; }

    public event Action<int, InventorySlot> OnSelectionChanged;

    public InventorySlot SelectedSlot
    {
        get
        {
            if (inventory == null ||
                SelectedIndex < 0 ||
                SelectedIndex >= inventory.Slots.Count)
            {
                return null;
            }

            return inventory.Slots[SelectedIndex];
        }
    }

    public ItemData SelectedItem
    {
        get
        {
            InventorySlot slot = SelectedSlot;

            return slot == null || slot.IsEmpty
                ? null
                : slot.Item;
        }
    }

    private void Start()
    {
        FindReferences();
        CreateSlots();

        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
            inventory.OnChanged += Refresh;
        }

        SelectSlot(0);
        Refresh();
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
        }
    }

    private void Update()
    {
        if (inventoryUI != null && inventoryUI.IsOpen)
        {
            return;
        }

        HandleNumberInput();
        HandleScrollInput();
    }

    private void FindReferences()
    {
        if (inventory == null)
        {
            inventory = PlayerInventory.Instance;
        }

        if (inventoryUI == null)
        {
            inventoryUI = FindAnyObjectByType<InventoryUI>();
        }
    }

    private void CreateSlots()
    {
        if (slotsContainer == null || slotPrefab == null)
        {
            Debug.LogError(
                "HotbarController: Missing UI references."
            );

            return;
        }

        for (int i = slotsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(slotsContainer.GetChild(i).gameObject);
        }

        slotViews.Clear();

        for (int i = 0; i < HotbarSize; i++)
        {
            HotbarSlotUI slotView = Instantiate(
                slotPrefab,
                slotsContainer
            );

            slotView.name = $"HotbarSlot_{i + 1}";
            slotView.Initialize(i + 1);

            slotViews.Add(slotView);
        }
    }

    private void HandleNumberInput()
    {
        for (int i = 0; i < HotbarSize; i++)
        {
            KeyCode key = KeyCode.Alpha1 + i;

            if (Input.GetKeyDown(key))
            {
                SelectSlot(i);
                return;
            }
        }
    }

    private void HandleScrollInput()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            SelectSlot(
                (SelectedIndex - 1 + HotbarSize) %
                HotbarSize
            );
        }
        else if (scroll < 0f)
        {
            SelectSlot(
                (SelectedIndex + 1) %
                HotbarSize
            );
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= HotbarSize)
        {
            return;
        }

        SelectedIndex = index;

        for (int i = 0; i < slotViews.Count; i++)
        {
            slotViews[i].SetSelected(i == SelectedIndex);
        }

        OnSelectionChanged?.Invoke(
            SelectedIndex,
            SelectedSlot
        );
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

            slotViews[i].SetSelected(i == SelectedIndex);
        }

        OnSelectionChanged?.Invoke(
            SelectedIndex,
            SelectedSlot
        );
    }
}