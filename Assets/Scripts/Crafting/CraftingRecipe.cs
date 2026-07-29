using UnityEngine;

[CreateAssetMenu(
    fileName = "NewRecipe",
    menuName = "Survival Game/Crafting/Recipe"
)]
public class CraftingRecipe : ScriptableObject
{
    [Header("Info")]
    public string recipeName;
    public Sprite icon;

    [Header("Ingredients")]
    public RecipeIngredient[] ingredients;

    [Header("Result")]
    public ItemData result;

    [Min(1)]
    public int resultAmount = 1;

    [Header("Crafting Station")]
    public CraftingStationType stationType =
        CraftingStationType.Basic;
}