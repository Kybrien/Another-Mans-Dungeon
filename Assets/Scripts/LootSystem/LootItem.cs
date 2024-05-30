using UnityEngine;

[System.Serializable]
public class LootItem
{
    public GameObject itemPrefab; // R�f�rence au prefab de l'arme
    public float dropChance; // La probabilit� que cet objet soit l�ch� (entre 0 et 1)
}
