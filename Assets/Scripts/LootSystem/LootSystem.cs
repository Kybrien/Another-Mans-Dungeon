using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public static LootSystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateLoot(Vector3 position)
    {
        LootTable lootTable = GetMonsterLootTable();
        if (lootTable != null)
        {
            List<LootItem> selectedLootItems = SelectLootItems(lootTable);
            foreach (LootItem selectedLootItem in selectedLootItems)
            {
                if (selectedLootItem != null)
                {
                    GameObject loot = Instantiate(selectedLootItem.itemPrefab, position, Quaternion.identity);
                    Rigidbody rb = loot.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = loot.AddComponent<Rigidbody>();
                    }
                    rb.isKinematic = false;
                    rb.detectCollisions = true;

                    switch (selectedLootItem.itemType)
                    {
                        case LootItem.ItemType.Weapon:
                            loot.tag = "Weapon";
                            break;
                        case LootItem.ItemType.Potion:
                            loot.tag = "Potion";
                            break;
                        case LootItem.ItemType.Armor:
                            loot.tag = "Armor";
                            break;
                        case LootItem.ItemType.Other:
                            loot.tag = "Other";
                            break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("LootTable not found.");
        }
    }

    private LootTable GetMonsterLootTable()
    {
        return Resources.Load<LootTable>("MonsterLootTable");
    }

    private List<LootItem> SelectLootItems(LootTable lootTable)
    {
        List<LootItem> selectedLootItems = new List<LootItem>();

        float noDropProbability = lootTable.noDropChance / 100f;
        if (Random.value < noDropProbability)
        {
            return selectedLootItems; 
        }

        bool weaponDropped = false;
        foreach (LootItem item in lootTable.lootItems)
        {
            float itemDropProbability = item.dropChance / 100f;
            if (Random.value < itemDropProbability)
            {
                if (item.itemType == LootItem.ItemType.Weapon)
                {
                    if (!weaponDropped)
                    {
                        selectedLootItems.Add(item);
                        weaponDropped = true;
                    }
                }
                else
                {
                    selectedLootItems.Add(item);
                }
            }
        }

        return selectedLootItems;
    }
}
