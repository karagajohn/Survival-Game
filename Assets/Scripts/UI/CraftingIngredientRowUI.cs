using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingIngredientRowUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private TMP_Text ingredientAmountText;
    [SerializeField] private TMP_Text ingredientStatusText;

    [Header("Status Colours")]
    [SerializeField] private Color availableColour =
        new Color(0.35f, 1f, 0.35f, 1f);

    [SerializeField] private Color missingColour =
        new Color(1f, 0.35f, 0.35f, 1f);

    private RecipeIngredient ingredient;
    private PlayerInventory inventory;

    public void Setup(
        RecipeIngredient newIngredient,
        PlayerInventory newInventory
    )
    {
        ingredient = newIngredient;
        inventory = newInventory;

        if (ingredient == null || ingredient.item == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (ingredientIcon != null)
        {
            ingredientIcon.sprite = ingredient.item.icon;
            ingredientIcon.enabled = ingredient.item.icon != null;
            ingredientIcon.preserveAspect = true;
        }

        if (ingredientNameText != null)
        {
            ingredientNameText.text =
                ingredient.item.displayName;
        }

        if (ingredientAmountText != null)
        {
            ingredientAmountText.text =
                $"x{ingredient.amount}";
        }

        Refresh();
    }

    public void Refresh()
    {
        if (ingredient == null ||
            ingredient.item == null ||
            inventory == null)
        {
            SetMissingState();
            return;
        }

        bool hasEnough = inventory.Contains(
            ingredient.item,
            ingredient.amount
        );

        Color statusColour =
            hasEnough ? availableColour : missingColour;

        if (ingredientAmountText != null)
        {
            ingredientAmountText.color = statusColour;
        }

        if (ingredientStatusText != null)
        {
            ingredientStatusText.text =
                hasEnough ? "OK" : "X";

            ingredientStatusText.color = statusColour;
        }
    }

    private void SetMissingState()
    {
        if (ingredientAmountText != null)
        {
            ingredientAmountText.color = missingColour;
        }

        if (ingredientStatusText != null)
        {
            ingredientStatusText.text = "X";
            ingredientStatusText.color = missingColour;
        }
    }
}