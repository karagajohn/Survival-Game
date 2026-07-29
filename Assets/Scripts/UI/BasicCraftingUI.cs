using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicCraftingUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Transform recipeContent;
    [SerializeField] private CraftingRecipeEntryUI recipeEntryPrefab;
    [SerializeField] private Button closeButton;

    [Header("Crafting References")]
    [SerializeField] private CraftingManager craftingManager;
    [SerializeField] private PlayerInventory inventory;

    [Header("Crafting Station")]
    [SerializeField]
    private CraftingStationType stationType =
        CraftingStationType.Basic;

    [SerializeField] private CraftingRecipe[] allRecipes;

    [Header("Input")]
    [SerializeField] private bool useToggleKey = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.C;

    [Header("Gameplay Scripts")]
    [SerializeField]
    private MonoBehaviour[] gameplayScriptsToDisable;

    private readonly List<CraftingRecipeEntryUI> generatedEntries =
        new List<CraftingRecipeEntryUI>();

    private bool isOpen;

    public bool IsOpen => isOpen;

    public CraftingStationType StationType => stationType;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = PlayerInventory.Instance;
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        BuildRecipeList();
        Close();
    }

    private void Update()
    {
        if (!useToggleKey)
        {
            return;
        }

        if (Input.GetKeyDown(toggleKey))
        {
            Toggle();
        }
    }

    private void BuildRecipeList()
    {
        if (recipeContent == null)
        {
            Debug.LogError(
                $"{name}: Recipe Content is missing."
            );

            return;
        }

        if (recipeEntryPrefab == null)
        {
            Debug.LogError(
                $"{name}: Recipe Entry Prefab is missing."
            );

            return;
        }

        if (craftingManager == null)
        {
            Debug.LogError(
                $"{name}: Crafting Manager is missing."
            );

            return;
        }

        ClearRecipeList();

        if (allRecipes == null)
        {
            return;
        }

        foreach (CraftingRecipe recipe in allRecipes)
        {
            if (recipe == null)
            {
                continue;
            }

            if (recipe.stationType != stationType)
            {
                continue;
            }

            CraftingRecipeEntryUI newEntry = Instantiate(
                recipeEntryPrefab,
                recipeContent
            );

            newEntry.Setup(
                recipe,
                craftingManager,
                this,
                inventory,
                stationType
            );

            generatedEntries.Add(newEntry);
        }
    }

    private void ClearRecipeList()
    {
        foreach (CraftingRecipeEntryUI entry in generatedEntries)
        {
            if (entry != null)
            {
                Destroy(entry.gameObject);
            }
        }

        generatedEntries.Clear();
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Open()
    {
        isOpen = true;

        if (craftingPanel != null)
        {
            craftingPanel.SetActive(true);
        }

        SetGameplayScriptsEnabled(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshAllEntries();
    }

    public void Close()
    {
        isOpen = false;

        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }

        SetGameplayScriptsEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshAllEntries()
    {
        foreach (CraftingRecipeEntryUI entry in generatedEntries)
        {
            if (entry != null)
            {
                entry.Refresh();
            }
        }
    }

    private void SetGameplayScriptsEnabled(
        bool shouldBeEnabled
    )
    {
        if (gameplayScriptsToDisable == null)
        {
            return;
        }

        foreach (MonoBehaviour gameplayScript
                 in gameplayScriptsToDisable)
        {
            if (gameplayScript != null)
            {
                gameplayScript.enabled =
                    shouldBeEnabled;
            }
        }
    }
}