using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeEntryUI : MonoBehaviour
{
    [Header("Recipe UI")]
    [SerializeField] private Image recipeIcon;
    [SerializeField] private TMP_Text recipeNameText;

    [Header("Ingredients")]
    [SerializeField] private Transform ingredientsContent;
    [SerializeField] private CraftingIngredientRowUI ingredientRowPrefab;

    [Header("Craft Button")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TMP_Text craftButtonText;

    private CraftingRecipe recipe;
    private CraftingManager craftingManager;
    private BasicCraftingUI parentUI;
    private PlayerInventory inventory;

    private CraftingStationType currentStationType;

    private readonly List<CraftingIngredientRowUI> ingredientRows =
        new List<CraftingIngredientRowUI>();

    public void Setup(
        CraftingRecipe newRecipe,
        CraftingManager newCraftingManager,
        BasicCraftingUI newParentUI,
        PlayerInventory newInventory,
        CraftingStationType newStationType
    )
    {
        recipe = newRecipe;
        craftingManager = newCraftingManager;
        parentUI = newParentUI;
        inventory = newInventory;
        currentStationType = newStationType;

        UpdateRecipeInformation();
        BuildIngredientRows();

        if (craftButton != null)
        {
            craftButton.onClick.RemoveAllListeners();
            craftButton.onClick.AddListener(TryCraft);
        }

        Refresh();
    }

    private void UpdateRecipeInformation()
    {
        if (recipe == null)
        {
            return;
        }

        if (recipeNameText != null)
        {
            recipeNameText.text =
                !string.IsNullOrWhiteSpace(recipe.recipeName)
                    ? recipe.recipeName
                    : recipe.name;
        }

        if (recipeIcon != null)
        {
            recipeIcon.sprite = recipe.icon;
            recipeIcon.enabled = recipe.icon != null;
            recipeIcon.preserveAspect = true;
        }
    }

    private void BuildIngredientRows()
    {
        ClearIngredientRows();

        if (recipe == null ||
            recipe.ingredients == null ||
            ingredientsContent == null ||
            ingredientRowPrefab == null)
        {
            return;
        }

        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            if (ingredient == null ||
                ingredient.item == null ||
                ingredient.amount <= 0)
            {
                continue;
            }

            CraftingIngredientRowUI newRow = Instantiate(
                ingredientRowPrefab,
                ingredientsContent
            );

            newRow.Setup(
                ingredient,
                inventory
            );

            ingredientRows.Add(newRow);
        }
    }

    private void ClearIngredientRows()
    {
        foreach (CraftingIngredientRowUI row in ingredientRows)
        {
            if (row != null)
            {
                Destroy(row.gameObject);
            }
        }

        ingredientRows.Clear();
    }

    public void Refresh()
    {
        foreach (CraftingIngredientRowUI row in ingredientRows)
        {
            if (row != null)
            {
                row.Refresh();
            }
        }

        bool canCraft =
            recipe != null &&
            craftingManager != null &&
            craftingManager.CanCraft(
                recipe,
                currentStationType
            );

        if (craftButton != null)
        {
            craftButton.interactable = canCraft;
        }

        if (craftButtonText != null)
        {
            craftButtonText.text =
                canCraft ? "CRAFT" : "MISSING";
        }
    }

    private void TryCraft()
    {
        if (recipe == null ||
            craftingManager == null)
        {
            return;
        }

        bool crafted = craftingManager.Craft(
            recipe,
            currentStationType
        );

        if (crafted && parentUI != null)
        {
            parentUI.RefreshAllEntries();
        }
    }
}