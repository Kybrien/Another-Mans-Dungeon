using UnityEngine;

[System.Serializable]
public class LootItem
{
    public enum ItemType
    {
        Weapon,
        Potion,
        Armor,
        Other
    }

    public GameObject itemPrefab; 
    public float dropChance; 
    public ItemType itemType; 
}