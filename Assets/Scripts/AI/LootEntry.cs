using System;
using UnityEngine;

[Serializable]
public class LootEntry
{
    public ItemData item;

    [Min(0f)]
    public float chance = 1f;
}