using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private HotbarController hotbarController;

    [SerializeField]
    private Transform heldItemPoint;

    private GameObject currentHeldObject;
    private ItemData currentItem;

    public ItemData CurrentItem => currentItem;
    

    private void Start()
    {
        if (hotbarController == null)
        {
            hotbarController =
                FindAnyObjectByType<HotbarController>();
        }

        if (hotbarController == null)
        {
            Debug.LogError(
                "EquipmentController: HotbarController is missing."
            );

            return;
        }

        hotbarController.OnSelectionChanged += HandleSelectionChanged;

        EquipItem(hotbarController.SelectedItem);
    }

    private void OnDestroy()
    {
        if (hotbarController != null)
        {
            hotbarController.OnSelectionChanged -= HandleSelectionChanged;
        }
    }

    private void HandleSelectionChanged(
        int selectedIndex,
        InventorySlot selectedSlot
    )
    {
        ItemData selectedItem = null;

        if (selectedSlot != null && !selectedSlot.IsEmpty)
        {
            selectedItem = selectedSlot.Item;
        }

        EquipItem(selectedItem);
    }

    private void EquipItem(ItemData item)
    {
        ClearHeldItem();

        currentItem = item;

        if (currentItem == null)
        {
            return;
        }

        if (currentItem.heldPrefab == null)
        {
            return;
        }

        if (heldItemPoint == null)
        {
            Debug.LogError(
                "EquipmentController: HeldItemPoint is missing."
            );

            return;
        }

        currentHeldObject = Instantiate(
            currentItem.heldPrefab,
            heldItemPoint
        );

        currentHeldObject.name =
            $"Held_{currentItem.displayName}";

        currentHeldObject.transform.localPosition =
        currentItem.heldPosition;

        currentHeldObject.transform.localEulerAngles =
        currentItem.heldRotation;

        currentHeldObject.transform.localScale =
        currentItem.heldScale;
    }

    private void ClearHeldItem()
    {
        if (currentHeldObject != null)
        {
            Destroy(currentHeldObject);
        }

        currentHeldObject = null;
        currentItem = null;
    }
}