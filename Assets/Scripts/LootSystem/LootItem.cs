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

    public GameObject itemPrefab; // Référence au prefab de l'objet
    public float dropChance; // La probabilité que cet objet soit lâché (entre 0 et 1)
    public ItemType itemType; // Le type de l'objet
}