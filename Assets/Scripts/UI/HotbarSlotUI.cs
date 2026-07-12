using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image background;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private TMP_Text numberText;

    [Header("Colors")]
    [SerializeField]
    private Color normalColor =
        new Color(0.18f, 0.18f, 0.22f, 1f);

    [SerializeField]
    private Color selectedColor =
        new Color(0.85f, 0.65f, 0.18f, 1f);

    public void Initialize(int slotNumber)
    {
        if (numberText != null)
        {
            numberText.text = slotNumber.ToString();
        }

        SetSelected(false);
        Clear();
    }

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

    public void SetSelected(bool selected)
    {
        if (background != null)
        {
            background.color =
                selected ? selectedColor : normalColor;
        }
    }
}