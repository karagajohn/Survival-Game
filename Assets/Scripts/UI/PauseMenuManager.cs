using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject controlsPanel;

    [SerializeField]
    private GameObject settingsPanel;

    [Header("Managers")]
    [SerializeField]
    private SettingsManager settingsManager;

    [Header("Player")]
    [SerializeField]
    private PlayerController playerController;

    private enum MenuState
    {
        Closed,
        Pause,
        Controls,
        Settings
    }

    private MenuState currentState =
        MenuState.Closed;

    private void Start()
    {
        if (playerController == null)
        {
            playerController =
                FindAnyObjectByType<PlayerController>();
        }

        CloseAllPanels();

        currentState =
            MenuState.Closed;

        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    // =========================================================
    // ESC
    // =========================================================

    private void HandleEscape()
    {
        switch (currentState)
        {
            case MenuState.Closed:
                OpenPauseMenu();
                break;

            case MenuState.Pause:
                ResumeGame();
                break;

            case MenuState.Controls:
                OpenPauseMenu();
                break;

            case MenuState.Settings:
                OpenPauseMenu();
                break;
        }
    }

    // =========================================================
    // PAUSE MENU
    // =========================================================

    public void OpenPauseMenu()
    {
        currentState =
            MenuState.Pause;

        CloseAllPanels();

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        DisablePlayerInput();

        UnlockCursor();
    }

    // =========================================================
    // CONTROLS
    // =========================================================

    public void OpenControls()
    {
        currentState =
            MenuState.Controls;

        CloseAllPanels();

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }

        DisablePlayerInput();

        UnlockCursor();
    }

    // =========================================================
    // SETTINGS
    // =========================================================

    public void OpenSettings()
    {
        currentState =
            MenuState.Settings;

        CloseAllPanels();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        DisablePlayerInput();

        UnlockCursor();
    }

    // =========================================================
    // BACK TO PAUSE
    // =========================================================

    public void BackToPause()
    {
        OpenPauseMenu();
    }

    // =========================================================
    // RESUME
    // =========================================================

    public void ResumeGame()
    {
        currentState =
            MenuState.Closed;

        CloseAllPanels();

        EnablePlayerInput();

        LockCursor();
    }

    // =========================================================
    // PANELS
    // =========================================================

    private void CloseAllPanels()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // =========================================================
    // PLAYER INPUT
    // =========================================================

    private void DisablePlayerInput()
    {
        if (playerController != null)
        {
            playerController.SetInputEnabled(false);
        }
    }

    private void EnablePlayerInput()
    {
        if (playerController != null)
        {
            playerController.SetInputEnabled(true);
        }
    }

    // =========================================================
    // CURSOR
    // =========================================================

    private void UnlockCursor()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    private void LockCursor()
    {
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }
}