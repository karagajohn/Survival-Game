using UnityEngine;

public class CraftingDebugTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CraftingManager craftingManager;

    [Header("Test Recipes")]
    [SerializeField]
    private CraftingRecipe stoneAxeRecipe;

    [SerializeField]
    private CraftingRecipe stonePickaxeRecipe;

    [SerializeField]
    private CraftingRecipe stoneSwordRecipe;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TryCraft(stoneAxeRecipe);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            TryCraft(stonePickaxeRecipe);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            TryCraft(stoneSwordRecipe);
        }
    }

    private void TryCraft(CraftingRecipe recipe)
    {
        if (craftingManager == null)
        {
            Debug.LogError(
                "CraftingDebugTest: CraftingManager is missing."
            );

            return;
        }

        craftingManager.Craft(recipe);
    }
}