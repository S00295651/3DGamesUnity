using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField] private List<ItemData> items = new();

    public event Action OnInventoryChanged;

    public void AddItem(ItemData newData)
    {
        items.Add(newData);
        OnInventoryChanged?.Invoke();
        Debug.Log($"Added {newData} to inventory.");
    }

    public bool HasItem(ItemData newData)
    {
        return items.Contains(newData);
    }

    // helper
    public float GetTotalBoost(StatType type)
    {
        float total = 0;
        foreach (var item in items)
        {
            if (item.affectedStat == type) total += item.attributeBoost;
        }
        return total;
    }
}
