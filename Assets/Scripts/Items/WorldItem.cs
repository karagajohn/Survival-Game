using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldItem : MonoBehaviour, IInteractable
{
    [Header("Item")]
    [SerializeField]
    private ItemData item;

    [Min(1)]
    [SerializeField]
    private int quantity = 1;

    [Header("Pickup")]
    [SerializeField]
    private bool destroyOnPickup = true;

    private bool hasBeenPickedUp;

    public ItemData Item => item;
    public int Quantity => quantity;

    public void Initialize(ItemData newItem, int newQuantity)
    {
        item = newItem;
        quantity = Mathf.Max(1, newQuantity);
        hasBeenPickedUp = false;

        UpdateObjectName();
    }

    public void Interact()
    {
        TryPickup();
    }

    public bool TryPickup()
    {
        if (hasBeenPickedUp)
        {
            return false;
        }

        if (item == null)
        {
            Debug.LogError(
                $"{gameObject.name}: WorldItem has no ItemData."
            );

            return false;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError(
                "WorldItem: PlayerInventory instance was not found."
            );

            return false;
        }

        bool addedSuccessfully =
            PlayerInventory.Instance.Add(item, quantity);

        if (!addedSuccessfully)
        {
            Debug.Log(
                $"Could not pick up {quantity}x {item.displayName}."
            );

            return false;
        }

        hasBeenPickedUp = true;

        Debug.Log(
            $"Picked up {quantity}x {item.displayName}."
        );

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }

        return true;
    }

    private void OnValidate()
    {
        quantity = Mathf.Max(1, quantity);
        UpdateObjectName();
    }

    private void UpdateObjectName()
    {
        if (item != null)
        {
            gameObject.name =
                $"WorldItem_{item.displayName}_x{quantity}";
        }
    }
}