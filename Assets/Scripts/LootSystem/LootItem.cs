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

    public GameObject itemPrefab; // R�f�rence au prefab de l'objet
    public float dropChance; // La probabilit� que cet objet soit l�ch� (entre 0 et 1)
    public ItemType itemType; // Le type de l'objet
}