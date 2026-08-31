using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings Panel")]
    [SerializeField]
    private GameObject settingsPanel;

    [Header("Sliders")]
    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider sensitivitySlider;

    [Header("Player")]
    [SerializeField]
    private PlayerController playerController;

    private const string MasterVolumeKey =
        "Settings_MasterVolume";

    private const string SensitivityKey =
        "Settings_MouseSensitivity";

    private const float DefaultVolume = 1f;
    private const float DefaultSensitivity = 2f;

    private const float MinimumSensitivity = 0.5f;
    private const float MaximumSensitivity = 10f;

    private void Start()
    {
        if (playerController == null)
        {
            playerController =
                FindAnyObjectByType<PlayerController>();
        }

        SetupSliders();

        LoadSettings();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // Safety:
        // Never leave the game paused if this object
        // is destroyed while the game is running.
        Time.timeScale = 1f;
    }

    // =========================================================
    // SETUP
    // =========================================================

    private void SetupSliders()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue =
                MinimumSensitivity;

            sensitivitySlider.maxValue =
                MaximumSensitivity;
        }
    }

    // =========================================================
    // SETTINGS PANEL
    // =========================================================

    public void ShowSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void HideSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // =========================================================
    // PAUSE
    // =========================================================

    public void PauseGame()
    {
        Time.timeScale = 0f;

        Debug.Log(
            "Game Paused. Time.timeScale = " +
            Time.timeScale
        );
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        Debug.Log(
            "Game Resumed. Time.timeScale = " +
            Time.timeScale
        );
    }

    public bool IsGamePaused()
    {
        return Time.timeScale <= 0f;
    }

    // =========================================================
    // MASTER VOLUME
    // =========================================================

    public void SetMasterVolume(float value)
    {
        value =
            Mathf.Clamp01(value);

        AudioListener.volume =
            value;

        PlayerPrefs.SetFloat(
            MasterVolumeKey,
            value
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Master Volume changed to: " +
            value
        );
    }

    // =========================================================
    // MOUSE SENSITIVITY
    // =========================================================

    public void SetMouseSensitivity(float value)
    {
        value =
            Mathf.Clamp(
                value,
                MinimumSensitivity,
                MaximumSensitivity
            );

        if (playerController == null)
        {
            Debug.LogWarning(
                "SettingsManager: PlayerController is missing."
            );

            return;
        }

        playerController.mouseSensitivity =
            value;

        PlayerPrefs.SetFloat(
            SensitivityKey,
            value
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Mouse Sensitivity changed to: " +
            value
        );
    }

    // =========================================================
    // LOAD SETTINGS
    // =========================================================

    private void LoadSettings()
    {
        float volume =
            PlayerPrefs.GetFloat(
                MasterVolumeKey,
                DefaultVolume
            );

        float sensitivity =
            PlayerPrefs.GetFloat(
                SensitivityKey,
                DefaultSensitivity
            );

        volume =
            Mathf.Clamp01(volume);

        sensitivity =
            Mathf.Clamp(
                sensitivity,
                MinimumSensitivity,
                MaximumSensitivity
            );

        // -----------------------------------------------------
        // APPLY VOLUME
        // -----------------------------------------------------

        AudioListener.volume =
            volume;

        // -----------------------------------------------------
        // APPLY SENSITIVITY
        // -----------------------------------------------------

        if (playerController != null)
        {
            playerController.mouseSensitivity =
                sensitivity;
        }

        // -----------------------------------------------------
        // UPDATE UI
        // -----------------------------------------------------

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(
                volume
            );
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.SetValueWithoutNotify(
                sensitivity
            );
        }
    }
}