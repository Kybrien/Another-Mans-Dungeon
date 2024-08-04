using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingTableUI; // Le Canvas de la table de craft
    public Image slotItem1;
    public Image slotBouteilleVide;
    public Image slotResult;

    public Button craftButton;


    private bool isCraftingTableOpen = false;

    void Start()
    {
        craftingTableUI.SetActive(false); // Assure que l'UI de craft est désactivée au début
    }

    void Update()
    {
        // Ajoute une logique pour détecter les clics sur le GameObject avec ce script
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == gameObject)
                {
                    ToggleCraftingTable();
                }
            }
        }
    }

    void ToggleCraftingTable()
    {
        if (isCraftingTableOpen)
        {
            CloseCraftingTable();
        }
        else
        {
            OpenCraftingTable();
        }
    }

    void OpenCraftingTable()
    {
        craftingTableUI.SetActive(true);
        isCraftingTableOpen = true;
    }

    void CloseCraftingTable()
    {
        craftingTableUI.SetActive(false);
        isCraftingTableOpen = false;
    }

}
