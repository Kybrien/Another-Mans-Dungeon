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
            foreach (LootItem lootItem in lootTable.lootItems)
            {
                if (Random.value <= lootItem.dropChance)
                {
                    GameObject loot = Instantiate(lootItem.itemPrefab, position, Quaternion.identity);
                    Rigidbody rb = loot.GetComponent<Rigidbody>();
                    if (rb == null)
                    {
                        rb = loot.AddComponent<Rigidbody>(); // Ajouter un Rigidbody si non existant
                    }
                    rb.isKinematic = false;
                    rb.detectCollisions = true;
                    loot.tag = "Weapon"; // Assurez-vous que l'objet est étiqueté correctement pour le ramassage
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
}
