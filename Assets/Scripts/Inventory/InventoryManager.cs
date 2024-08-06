using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool isStorageOpened;

    [SerializeField] private GameObject[] hotbarSlots = new GameObject[4];
    [SerializeField] private GameObject[] slots = new GameObject[20];
    [SerializeField] private GameObject inventoryParent;
    [SerializeField] private GameObject storageParent;
    [SerializeField] private Transform handParent;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Camera cam;

    public GameObject[] Slots => slots;
    public GameObject ItemPrefab => itemPrefab;

    GameObject draggedObject;
    GameObject lastItemSlot;

    Storage lastStorage;

    bool isInventoryOpened;

    int selectedHotbarSlot = 0;

    void Start()
    {
        HotbarItemChanged();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckForHotbarInput();

        storageParent.SetActive(isStorageOpened);
        inventoryParent.SetActive(isInventoryOpened);

        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isInventoryOpened)
            {
                Cursor.lockState = CursorLockMode.Locked;
                isInventoryOpened = false;
                isStorageOpened = false;

                if (lastStorage != null)
                {
                    CloseStorage(lastStorage);
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInventoryOpened = true;
            }
        }
    }

    private void CheckForHotbarInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedHotbarSlot = 0;
            HotbarItemChanged();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedHotbarSlot = 1;
            HotbarItemChanged();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedHotbarSlot = 2;
            HotbarItemChanged();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedHotbarSlot = 3;
            HotbarItemChanged();
        }
    }

    private void HotbarItemChanged()
    {
        // Désactivez tous les enfants de handParent
        foreach (Transform child in handParent)
        {
            Destroy(child.gameObject);
        }

        // Sélectionnez le slot actif de la hotbar
        InventorySlot selectedSlot = hotbarSlots[selectedHotbarSlot].GetComponent<InventorySlot>();
        if (selectedSlot != null && selectedSlot.HeldItem != null)
        {
            InventoryItem heldItem = selectedSlot.HeldItem.GetComponent<InventoryItem>();
            if (heldItem != null)
            {
                InstantiateItemInHand(heldItem);
            }
            else
            {
                Debug.LogWarning("L'item détenu n'a pas de composant InventoryItem.");
            }
        }
        else
        {
            Debug.LogWarning("Le slot sélectionné n'a pas d'item ou est nul.");
        }

        // Mettre à jour la mise à l'échelle des slots de la hotbar
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            Vector3 scale = i == selectedHotbarSlot ? new Vector3(1.1f, 1.1f, 1.1f) : new Vector3(0.9f, 0.9f, 0.9f);
            hotbarSlots[i].transform.localScale = scale;
        }
    }



    private void InstantiateItemInHand(InventoryItem item)
    {
        // Créez une instance du prefab en tant qu'enfant de handParent
        GameObject itemInHand = Instantiate(itemPrefab, handParent);
        InventoryItem handItemComponent = itemInHand.GetComponent<InventoryItem>();

        // Configurez les propriétés du nouvel item
        handItemComponent.itemScriptableObject = item.itemScriptableObject;
        handItemComponent.stackCurrent = item.stackCurrent;

        // Ajustez la position, la rotation et l'échelle de l'item
        itemInHand.transform.localPosition = Vector3.zero; // Position relative à handParent
        itemInHand.transform.localRotation = Quaternion.identity; // Rotation relative à handParent
        itemInHand.transform.localScale = Vector3.one; // Échelle relative à handParent

        // Assurez-vous que l'item est actif
        itemInHand.SetActive(true);

        // Vérifiez les composants du prefab
        if (itemInHand.GetComponent<Renderer>() == null)
        {
            Debug.LogError("Le prefab n'a pas de composant Renderer !");
        }
        else
        {
            Debug.Log("Le prefab a un composant Renderer.");
        }

        // Affichez les détails sur la position et l'état
        Debug.Log("Item instancié dans la main: " + itemInHand.name);
        Debug.Log("Position de l'item: " + itemInHand.transform.position);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            // Il y a un item dans le slot - on le prend
            if (slot != null && slot.HeldItem != null)
            {
                draggedObject = slot.HeldItem;
                slot.HeldItem = null;
                lastItemSlot = clickedObject;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot inventorySlot = clickedObject.GetComponent<InventorySlot>();
            CraftingSlot craftingSlot = clickedObject.GetComponent<CraftingSlot>();

            // Si on relâche l'item sur un slot de crafting vide
            if (craftingSlot != null && craftingSlot.IsEmpty())
            {
                craftingSlot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(craftingSlot.transform);
            }
            // Si on relâche l'item sur un slot d'inventaire vide
            else if (inventorySlot != null && inventorySlot.IsEmpty())
            {
                inventorySlot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(inventorySlot.transform.parent.parent.GetChild(2));
            }
            // Si on relâche l'item sur un slot d'inventaire qui contient déjà un item
            else if (inventorySlot != null && !inventorySlot.IsEmpty())
            {
                GameObject tempItem = inventorySlot.HeldItem;
                inventorySlot.SetHeldItem(draggedObject);
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(tempItem);
                tempItem.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));
                draggedObject.transform.SetParent(inventorySlot.transform.parent.parent.GetChild(2));
            }
            // Si on relâche l'item sur un slot de crafting qui contient déjà un item
            else if (craftingSlot != null && !craftingSlot.IsEmpty())
            {
                GameObject tempItem = craftingSlot.HeldItem;
                craftingSlot.SetHeldItem(draggedObject);
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(tempItem);
                tempItem.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));
                draggedObject.transform.SetParent(craftingSlot.transform);
            }
            // Si on relâche l'item ailleurs (retourne l'item au dernier slot)
            else
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(lastItemSlot.transform.parent.parent.GetChild(2));
            }

            draggedObject = null;
        }
    }

    public void ItemPicked(GameObject pickedItem)
    {
        GameObject emptySlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot.HeldItem == null)
            {
                emptySlot = slots[i];
                break;
            }
        }

        if (emptySlot != null)
        {
            GameObject newItem = Instantiate(itemPrefab);
            newItem.GetComponent<InventoryItem>().itemScriptableObject = pickedItem.GetComponent<itemPickable>().itemScriptableObject;
            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
            newItem.GetComponent<InventoryItem>().stackCurrent = 1;

            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);

            Destroy(pickedItem);
        }
    }

    public void OpenStorage(Storage storage)
    {
        lastStorage = storage;

        Cursor.lockState = CursorLockMode.None;
        isStorageOpened = true;

        for (int i = 0; i < storageParent.transform.GetChild(1).childCount; i++)
        {
            storageParent.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < storage.size; i++)
        {
            storageParent.transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
        }
        float sizeY = (float)Mathf.CeilToInt(storage.size / 4f) / 4;
        storageParent.transform.GetChild(0).localScale = new Vector2(1, sizeY);

        float posY = (1 - sizeY) * 230;
        storageParent.transform.GetChild(0).localPosition = new Vector2(-615, 130 + posY);

        for (int i = 0; i < storageParent.transform.GetChild(2).childCount; i++)
        {
            Destroy(storageParent.transform.GetChild(2).GetChild(i).gameObject);
        }

        int index = 0;
        foreach (StorageItem storageItem in storage.items)
        {
            if (storageItem.itemScriptableObject != null)
            {
                GameObject newItem = Instantiate(itemPrefab);
                InventoryItem item = newItem.GetComponent<InventoryItem>();
                item.itemScriptableObject = storageItem.itemScriptableObject;
                item.stackCurrent = storageItem.currentStack;

                Transform slot = storageParent.transform.GetChild(1).GetChild(index);
                newItem.transform.SetParent(slot.parent.parent.GetChild(2));
                slot.GetComponent<InventorySlot>().SetHeldItem(newItem);
                newItem.transform.localScale = new Vector3(1, 1, 1);
            }
            index++;
        }
    }

    public void CloseStorage(Storage storage)
    {
        lastStorage = null;
        Cursor.lockState = CursorLockMode.Locked;
        isStorageOpened = false;

        Transform slotsParent = storageParent.transform.GetChild(1);

        storage.items.Clear();

        for (int i = 0; i < slotsParent.childCount; i++)
        {
            Transform slot = slotsParent.GetChild(i);

            if (slot.gameObject.activeInHierarchy && slot.GetComponent<InventorySlot>().HeldItem != null)
            {
                InventoryItem inventoryItem = slot.GetComponent<InventorySlot>().HeldItem.GetComponent<InventoryItem>();

                storage.items.Add(new StorageItem(inventoryItem.stackCurrent, inventoryItem.itemScriptableObject));
            }
            else
            {
                storage.items.Add(new StorageItem(0, null));
            }
        }
    }

    public void OnCraftButtonPressed()
    {
        CraftingManager craftingManager = FindObjectOfType<CraftingManager>();
        if (craftingManager != null)
        {
            craftingManager.AttemptCraft();
        }
    }
}
