using UnityEngine;

public enum ItemCategory { Food, Tool, Animal, Plant }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemCategory category;
    public float weight;
    public bool special;

    // Optional unique ID if needed, otherwise fallback to name-based equality
    [SerializeField] private string itemID;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = System.Guid.NewGuid().ToString(); // auto-assign if missing
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is ItemData other)
        {
            return itemID == other.itemID;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return itemID != null ? itemID.GetHashCode() : base.GetHashCode();
    }
}
