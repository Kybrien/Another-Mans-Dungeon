using UnityEngine;

public class HerbQuest : MonoBehaviour
{
    public QuestManager questManager; // Assign in the Inspector
    public int questIndex; // Indice de la qu�te � mettre � jour, initialis� � 1

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (questManager != null && questIndex >= 0 && questIndex < questManager.quests.Count)
            {
                Quest quest = questManager.quests[questIndex];
                if (quest != null && quest.isActive)
                {
                    questManager.UpdateQuestProgress(1);
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("Invalid quest index!");
            }
        }
    }
}
