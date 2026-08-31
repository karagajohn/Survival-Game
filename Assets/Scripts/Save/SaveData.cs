using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // =========================================================
    // SAVE INFORMATION
    // =========================================================

    public string saveVersion = "1.0";

    public string saveDate;

    // =========================================================
    // PLAYER POSITION
    // =========================================================

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public float playerRotationX;
    public float playerRotationY;
    public float playerRotationZ;

    // =========================================================
    // PLAYER STATS
    // =========================================================

    public float health;
    public float hunger;
    public float stamina;

    // =========================================================
    // INVENTORY
    // =========================================================

    public List<InventorySaveData> inventory =
        new List<InventorySaveData>();
}

[Serializable]
public class InventorySaveData
{
    public string itemId;

    public int quantity;
}