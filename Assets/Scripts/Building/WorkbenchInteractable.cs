using UnityEngine;

public class WorkbenchInteractable : MonoBehaviour, IInteractable
{
    [Header("Crafting Station")]
    [SerializeField]
    private CraftingStationType stationType =
        CraftingStationType.Workbench;

    private BasicCraftingUI craftingUI;

    public void Interact()
    {
        if (craftingUI == null)
        {
            craftingUI = FindCraftingUI();
        }

        if (craftingUI == null)
        {
            Debug.LogError(
                $"WorkbenchInteractable: No crafting UI was found " +
                $"for station type {stationType}."
            );

            return;
        }

        craftingUI.Open();
    }

    private BasicCraftingUI FindCraftingUI()
    {
        BasicCraftingUI[] craftingUIs =
            Object.FindObjectsByType<BasicCraftingUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (BasicCraftingUI ui in craftingUIs)
        {
            if (ui != null &&
                ui.StationType == stationType)
            {
                return ui;
            }
        }

        return null;
    }
}