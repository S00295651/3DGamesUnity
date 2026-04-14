using UnityEngine;

public enum StatType { groundSpeed, airSpeed, JumpForce }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public StatType affectedStat;
    public float attributeBoost;
}
