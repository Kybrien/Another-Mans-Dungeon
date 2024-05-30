using UnityEngine;

public class MonsterKiller : MonoBehaviour
{
    public float interactRange = 3.0f; // Distance maximale pour interagir avec un monstre

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryKillMonster();
        }
    }

    void TryKillMonster()
    {
        // Trouver tous les colliders dans la port�e d'interaction
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        bool monsterFound = false;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Monster"))
            {
                KillMonster(collider.gameObject);
                monsterFound = true;
                break;
            }
        }

        if (!monsterFound)
        {
            Debug.Log("No monster found within range.");
        }
    }

    void KillMonster(GameObject monster)
    {
        Debug.Log("Monster killed: " + monster.name);
        // Ajouter la logique de "mort" ici (par exemple d�sactiver le monstre)
        monster.SetActive(false);
        // Lancer le syst�me de loot
        LootSystem.Instance.GenerateLoot(monster.transform.position);
    }
}
