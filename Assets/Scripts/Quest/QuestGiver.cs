using UnityEngine;
using Mirror;

public class QuestGiver : NetworkBehaviour
{
    public GameObject dialoguePanel; // Assign in the Inspector
    public GameObject questSteps; // Assign in the Inspector
    public GameObject rewardDialogue; // Assign in the Inspector
    public GameObject npc; // Assign in the Inspector
    public QuestManager questManager; // Assign in the Inspector
    public BoxCollider boxCollider; // Assign in the Inspector
    public GameObject weaponPrefab; // Assigner dans l'Inspecteur

    private bool playerInRange = false;

    [SyncVar(hook = nameof(OnQuestAcceptedChanged))]
    private bool questAccepted = false;

    public int questIndex; // L'index de la quête à accepter, assigné dans l'inspecteur

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.A))
        {
            if (questAccepted)
            {
                if (questManager.currentQuest != null && questManager.currentQuest.isComplete)
                {
                    CmdEnableCollider();
                }
            }
            else
            {
                CmdAcceptQuest();
            }
        }
    }

    [Command]
    private void CmdEnableCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("Player") && !questAccepted)
        {
            Debug.Log("Player entered NPC trigger zone");
            dialoguePanel.SetActive(true);
            playerInRange = true;
        }

        if (other.CompareTag("Player") && questAccepted && questManager.currentQuest != null && questManager.currentQuest.isComplete)
        {
            rewardDialogue.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited NPC trigger zone");
            dialoguePanel.SetActive(false);
            playerInRange = false;
        }
    }

    [Command]
    public void CmdAcceptQuest()
    {
        Debug.Log("Quest accepted");

        if (dialoguePanel == null)
        {
            Debug.LogError("dialoguePanel is not assigned!");
            return;
        }

        if (questSteps == null)
        {
            Debug.LogError("questSteps is not assigned!");
            return;
        }

        if (questManager == null)
        {
            Debug.LogError("questManager is not assigned!");
            return;
        }

        if (questIndex < 0 || questIndex >= questManager.quests.Count)
        {
            Debug.LogError("Invalid quest index! Quest index: " + questIndex);
            return;
        }

        questAccepted = true;
        questManager.StartQuest(questIndex);
        RpcUpdateQuestUI();
    }

    private void OnQuestAcceptedChanged(bool oldQuestAccepted, bool newQuestAccepted)
    {
        if (newQuestAccepted)
        {
            dialoguePanel.SetActive(false);
            questSteps.SetActive(true);

            if (boxCollider != null)
            {
                boxCollider.enabled = false;
            }
        }
    }

    [ClientRpc]
    private void RpcUpdateQuestUI()
    {
        questManager.UpdateQuestUI();
    }

    [Command]
    public void CmdClaimReward()
    {
        Debug.Log("Reward claimed");

        if (questManager.currentQuest != null && questManager.currentQuest.isComplete && weaponPrefab != null)
        {
            GameObject weapon = Instantiate(weaponPrefab, transform.position + transform.forward * 2, Quaternion.identity);
            NetworkServer.Spawn(weapon);
        }

        RpcHideQuestUI();

        if (boxCollider != null) boxCollider.enabled = false;
        if (npc != null) npc.SetActive(false);
    }

    [ClientRpc]
    private void RpcHideQuestUI()
    {
        dialoguePanel.SetActive(false);
        questSteps.SetActive(false);
        rewardDialogue.SetActive(false);
    }
}
