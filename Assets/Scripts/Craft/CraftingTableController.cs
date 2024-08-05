using UnityEngine;

public class CraftingTableController : MonoBehaviour
{
    [SerializeField] private GameObject craftingPanel; // R�f�rence au panel de crafting
    [SerializeField] private Transform player; // R�f�rence � la position du joueur
    [SerializeField] private float interactionDistance = 3f; // Distance maximale pour l'interaction

    private void Start()
    {
        // D�sactiver le panel de crafting au d�but du jeu
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryOpenCraftingPanel();
        }
    }

    private void TryOpenCraftingPanel()
    {
        // V�rifiez si le joueur est � proximit� de la table de craft
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance)
        {
            ToggleCraftingPanel();
        }
    }

    private void ToggleCraftingPanel()
    {
        if (craftingPanel != null)
        {
            bool isActive = craftingPanel.activeSelf;
            craftingPanel.SetActive(!isActive);
            Debug.Log($"Crafting panel is now {(isActive ? "closed" : "open")}");
        }
        else
        {
            Debug.LogWarning("Crafting panel reference is not assigned.");
        }
    }
}
