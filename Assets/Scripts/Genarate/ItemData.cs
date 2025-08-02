using UnityEngine;

public enum ItemCategory { Food, Tool, Animal,  }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemCategory category;
    public float weight;
}
