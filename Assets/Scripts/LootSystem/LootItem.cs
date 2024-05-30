using UnityEngine;

[System.Serializable]
public class LootItem
{
    public GameObject itemPrefab; // Référence au prefab de l'arme
    public float dropChance; // La probabilité que cet objet soit lâché (entre 0 et 1)
}
