using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // =========================================================
    // SAVE SETTINGS
    // =========================================================

    [Header("Save Settings")]
    [Min(1f)]
    [SerializeField]
    private float autoSaveInterval = 300f;

    [SerializeField]
    private bool autoSaveEnabled = true;

    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private PlayerInventory playerInventory;

    [SerializeField]
    private ItemDatabase itemDatabase;

    // =========================================================
    // INTERNAL
    // =========================================================

    private float nextAutoSaveTime;

    private const string SaveFileName =
        "survival_save.json";

    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            SaveFileName
        );

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        FindReferences();

        ResetAutoSaveTimer();
    }

    private void Start()
    {
        FindReferences();

        // -----------------------------------------------------
        // LOAD PREVIOUS SAVE
        // -----------------------------------------------------

        if (HasSave())
        {
            Debug.Log(
                "SaveManager: Save file found. " +
                "Loading previous game..."
            );

            LoadGame();
        }
        else
        {
            Debug.Log(
                "SaveManager: No previous save found. " +
                "Starting new game."
            );
        }
    }

    private void Update()
    {
        if (!autoSaveEnabled)
        {
            return;
        }

        if (Time.time <
            nextAutoSaveTime)
        {
            return;
        }

        SaveGame();

        ResetAutoSaveTimer();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    // =========================================================
    // REFERENCES
    // =========================================================

    private void FindReferences()
    {
        if (playerStats == null)
        {
            playerStats =
                FindAnyObjectByType<PlayerStats>();
        }

        if (playerInventory == null)
        {
            playerInventory =
                FindAnyObjectByType<PlayerInventory>();
        }
    }

    // =========================================================
    // SAVE
    // =========================================================

    public void SaveGame()
    {
        FindReferences();

        if (playerStats == null)
        {
            Debug.LogError(
                "SaveManager: PlayerStats not found."
            );

            return;
        }

        if (playerInventory == null)
        {
            Debug.LogError(
                "SaveManager: PlayerInventory not found."
            );

            return;
        }

        if (itemDatabase == null)
        {
            Debug.LogError(
                "SaveManager: ItemDatabase is missing."
            );

            return;
        }

        SaveData data =
            CreateSaveData();

        string json =
            JsonUtility.ToJson(
                data,
                true
            );

        try
        {
            File.WriteAllText(
                SavePath,
                json
            );

            Debug.Log(
                "Game saved successfully.\n" +
                "Path: " +
                SavePath
            );
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "SaveManager: Failed to save game.\n" +
                exception
            );
        }

        ResetAutoSaveTimer();
    }

    // =========================================================
    // CREATE SAVE DATA
    // =========================================================

    private SaveData CreateSaveData()
    {
        SaveData data =
            new SaveData();

        data.saveDate =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss"
            );

        // -----------------------------------------------------
        // PLAYER TRANSFORM
        // -----------------------------------------------------

        Transform playerTransform =
            playerStats.transform;

        Vector3 position =
            playerTransform.position;

        Vector3 rotation =
            playerTransform.eulerAngles;

        data.playerPositionX =
            position.x;

        data.playerPositionY =
            position.y;

        data.playerPositionZ =
            position.z;

        data.playerRotationX =
            rotation.x;

        data.playerRotationY =
            rotation.y;

        data.playerRotationZ =
            rotation.z;

        // -----------------------------------------------------
        // PLAYER STATS
        // -----------------------------------------------------

        data.health =
            playerStats.health;

        data.hunger =
            playerStats.hunger;

        data.stamina =
            playerStats.stamina;

        // -----------------------------------------------------
        // INVENTORY
        // -----------------------------------------------------

        data.inventory.Clear();

        foreach (
            InventorySlot slot
            in playerInventory.Slots
        )
        {
            if (slot == null ||
                slot.IsEmpty ||
                slot.Item == null)
            {
                data.inventory.Add(
                    new InventorySaveData
                    {
                        itemId = "",
                        quantity = 0
                    }
                );

                continue;
            }

            data.inventory.Add(
                new InventorySaveData
                {
                    itemId =
                        slot.Item.itemId,

                    quantity =
                        slot.Quantity
                }
            );
        }

        return data;
    }

    // =========================================================
    // LOAD
    // =========================================================

    public bool LoadGame()
    {
        FindReferences();

        if (playerStats == null)
        {
            Debug.LogError(
                "SaveManager: PlayerStats not found."
            );

            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError(
                "SaveManager: PlayerInventory not found."
            );

            return false;
        }

        if (itemDatabase == null)
        {
            Debug.LogError(
                "SaveManager: ItemDatabase is missing."
            );

            return false;
        }

        if (!File.Exists(SavePath))
        {
            Debug.Log(
                "SaveManager: No save file found."
            );

            return false;
        }

        try
        {
            string json =
                File.ReadAllText(
                    SavePath
                );

            SaveData data =
                JsonUtility.FromJson<SaveData>(
                    json
                );

            if (data == null)
            {
                Debug.LogError(
                    "SaveManager: Save data is invalid."
                );

                return false;
            }

            ApplySaveData(data);

            Debug.Log(
                "Game loaded successfully."
            );

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "SaveManager: Failed to load game.\n" +
                exception
            );

            return false;
        }
    }

    // =========================================================
    // APPLY SAVE DATA
    // =========================================================

    private void ApplySaveData(
        SaveData data
    )
    {
        // -----------------------------------------------------
        // PLAYER TRANSFORM
        // -----------------------------------------------------

        Transform playerTransform =
            playerStats.transform;

        Vector3 position =
            new Vector3(
                data.playerPositionX,
                data.playerPositionY,
                data.playerPositionZ
            );

        Vector3 rotation =
            new Vector3(
                data.playerRotationX,
                data.playerRotationY,
                data.playerRotationZ
            );

        CharacterController characterController =
            playerStats.GetComponent<CharacterController>();

        if (characterController != null)
        {
            characterController.enabled =
                false;
        }

        playerTransform.SetPositionAndRotation(
            position,
            Quaternion.Euler(rotation)
        );

        if (characterController != null)
        {
            characterController.enabled =
                true;
        }

        // -----------------------------------------------------
        // PLAYER STATS
        // -----------------------------------------------------

        playerStats.health =
            Mathf.Clamp(
                data.health,
                0f,
                playerStats.maxHealth
            );

        playerStats.hunger =
            Mathf.Clamp(
                data.hunger,
                0f,
                playerStats.maxHunger
            );

        playerStats.stamina =
            Mathf.Clamp(
                data.stamina,
                0f,
                playerStats.maxStamina
            );

        // -----------------------------------------------------
        // INVENTORY
        // -----------------------------------------------------

        LoadInventory(
            data
        );

        // -----------------------------------------------------
        // UPDATE PLAYER UI
        // -----------------------------------------------------

        playerStats.SendMessage(
            "NotifyChanged",
            SendMessageOptions.DontRequireReceiver
        );
    }

    // =========================================================
    // LOAD INVENTORY
    // =========================================================

    private void LoadInventory(
        SaveData data
    )
    {
        if (data.inventory == null)
        {
            return;
        }

        // -----------------------------------------------------
        // CLEAR CURRENT INVENTORY
        // -----------------------------------------------------

        playerInventory.ClearAll();

        // -----------------------------------------------------
        // RESTORE SAVED SLOTS
        // -----------------------------------------------------

        int slotsToLoad =
            Mathf.Min(
                data.inventory.Count,
                playerInventory.Capacity
            );

        for (
            int i = 0;
            i < slotsToLoad;
            i++
        )
        {
            InventorySaveData savedSlot =
                data.inventory[i];

            if (savedSlot == null ||
                string.IsNullOrWhiteSpace(
                    savedSlot.itemId
                ) ||
                savedSlot.quantity <= 0)
            {
                continue;
            }

            ItemData item =
                itemDatabase.GetItem(
                    savedSlot.itemId
                );

            if (item == null)
            {
                Debug.LogWarning(
                    "SaveManager: Could not find " +
                    $"ItemData with itemId " +
                    $"'{savedSlot.itemId}'."
                );

                continue;
            }

            playerInventory.SetSlot(
                i,
                item,
                savedSlot.quantity
            );
        }

        playerInventory.NotifyChanged();
    }

    // =========================================================
    // SAVE FILE
    // =========================================================

    public bool HasSave()
    {
        return File.Exists(
            SavePath
        );
    }

    public void DeleteSave()
    {
        if (!File.Exists(SavePath))
        {
            return;
        }

        try
        {
            File.Delete(
                SavePath
            );

            Debug.Log(
                "SaveManager: Save deleted."
            );
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "SaveManager: Failed to delete save.\n" +
                exception
            );
        }
    }

    // =========================================================
    // AUTO SAVE
    // =========================================================

    private void ResetAutoSaveTimer()
    {
        nextAutoSaveTime =
            Time.time +
            autoSaveInterval;
    }

    public void SetAutoSaveEnabled(
        bool enabled
    )
    {
        autoSaveEnabled =
            enabled;

        if (enabled)
        {
            ResetAutoSaveTimer();
        }
    }

    // =========================================================
    // DEBUG / INFORMATION
    // =========================================================

    public string GetSavePath()
    {
        return SavePath;
    }

    public float GetAutoSaveInterval()
    {
        return autoSaveInterval;
    }
}