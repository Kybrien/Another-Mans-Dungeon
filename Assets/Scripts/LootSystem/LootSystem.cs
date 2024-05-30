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
            LootItem selectedLootItem = SelectLootItem(lootTable);
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
                loot.tag = "Weapon"; 
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

    private LootItem SelectLootItem(LootTable lootTable)
    {
        float totalWeight = 0f;
        foreach (LootItem item in lootTable.lootItems)
        {
            totalWeight += item.dropChance;
        }

        float randomValue = Random.value * totalWeight;
        float cumulativeWeight = 0f;

        foreach (LootItem item in lootTable.lootItems)
        {
            cumulativeWeight += item.dropChance;
            if (randomValue <= cumulativeWeight)
            {
                return item;
            }
        }

        return null; 
    }
}
