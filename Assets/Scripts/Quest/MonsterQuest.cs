using UnityEngine;
using Mirror;

public class MonsterQuest : NetworkBehaviour
{
    public QuestManager questManager; // Assignez dans l'inspecteur
    public int questIndex; // Indice de la qu�te � mettre � jour

    private void OnDestroy()
    {
        if (!isServer) return; // Seul le serveur g�re les mises � jour de qu�te

        // V�rifier que l'indice de qu�te est valide et que le gestionnaire de qu�tes est d�fini
        if (questManager != null && questIndex >= 0 && questIndex < questManager.quests.Count)
        {
            Quest quest = questManager.quests[questIndex];
            if (quest != null && quest.isActive)
            {
                // Commande pour mettre � jour la progression de la qu�te sur le serveur
                CmdUpdateQuestProgress(1);
            }
        }
        else
        {
            Debug.LogError("Invalid quest index or QuestManager not set!");
        }
    }

    [Command]
    private void CmdUpdateQuestProgress(int amount)
    {
        // V�rifier � nouveau que l'indice de qu�te est valide avant de mettre � jour
        if (questIndex >= 0 && questIndex < questManager.quests.Count)
        {
            questManager.UpdateQuestProgress(amount);
        }
        else
        {
            Debug.LogError("Invalid quest index in CmdUpdateQuestProgress!");
        }
    }
}
