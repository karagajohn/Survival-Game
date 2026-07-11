using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TMP_Text quantityText;

    public void Display(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            Clear();
            return;
        }

        ItemData item = slot.Item;

        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = item.icon != null;
            itemIcon.preserveAspect = true;
        }

        if (quantityText != null)
        {
            bool showQuantity = slot.Quantity > 1;

            quantityText.gameObject.SetActive(showQuantity);
            quantityText.text = slot.Quantity.ToString();
        }
    }

    public void Clear()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (quantityText != null)
        {
            quantityText.text = string.Empty;
            quantityText.gameObject.SetActive(false);
        }
    }
}