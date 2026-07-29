using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerInventory inventory;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = PlayerInventory.Instance;
        }

        if (inventory == null)
        {
            Debug.LogError(
                "CraftingManager: PlayerInventory was not found."
            );
        }
    }

    public bool CanCraft(
        CraftingRecipe recipe,
        CraftingStationType currentStation =
            CraftingStationType.Basic
    )
    {
        if (recipe == null)
        {
            return false;
        }

        if (inventory == null)
        {
            return false;
        }

        if (recipe.stationType != currentStation)
        {
            return false;
        }

        if (recipe.result == null)
        {
            return false;
        }

        if (recipe.resultAmount <= 0)
        {
            return false;
        }

        if (recipe.ingredients == null)
        {
            return true;
        }

        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            if (ingredient == null)
            {
                continue;
            }

            if (ingredient.item == null)
            {
                continue;
            }

            if (ingredient.amount <= 0)
            {
                continue;
            }

            if (!inventory.Contains(
                    ingredient.item,
                    ingredient.amount))
            {
                return false;
            }
        }

        return true;
    }

    public bool Craft(
        CraftingRecipe recipe,
        CraftingStationType currentStation =
            CraftingStationType.Basic
    )
    {
        if (!CanCraft(recipe, currentStation))
        {
            Debug.Log(
                $"Cannot craft {GetRecipeName(recipe)} " +
                $"at {currentStation}."
            );

            return false;
        }

        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            if (ingredient == null)
            {
                continue;
            }

            if (ingredient.item == null)
            {
                continue;
            }

            if (ingredient.amount <= 0)
            {
                continue;
            }

            bool removed = inventory.Remove(
                ingredient.item,
                ingredient.amount
            );

            if (!removed)
            {
                Debug.LogError(
                    $"Crafting failed while removing " +
                    $"{ingredient.amount}x " +
                    $"{ingredient.item.displayName}."
                );

                return false;
            }
        }

        bool added = inventory.Add(
            recipe.result,
            recipe.resultAmount
        );

        if (!added)
        {
            Debug.LogWarning(
                $"Crafted {recipe.resultAmount}x " +
                $"{recipe.result.displayName}, " +
                "but the inventory could not fit the result."
            );

            return false;
        }

        Debug.Log(
            $"Crafted {recipe.resultAmount}x " +
            $"{recipe.result.displayName}."
        );

        return true;
    }

    public bool CanCraftAtStation(
        CraftingRecipe recipe,
        CraftingStationType stationType
    )
    {
        if (recipe == null)
        {
            return false;
        }

        return recipe.stationType == stationType;
    }

    private string GetRecipeName(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            return "Unknown Recipe";
        }

        if (!string.IsNullOrWhiteSpace(recipe.recipeName))
        {
            return recipe.recipeName;
        }

        return recipe.name;
    }
}