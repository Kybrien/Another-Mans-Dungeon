using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool isStorageOpened;

    [SerializeField] GameObject[] hotbarSlots = new GameObject[4];
    [SerializeField] GameObject[] slots = new GameObject[20];
    [SerializeField] GameObject inventoryParent;
    [SerializeField] Transform handParent;


    [SerializeField] GameObject itemPrefab;
    [SerializeField] Camera cam;

    GameObject draggedObject;
    GameObject lastItemSlot;
    bool isInventoryOpened;
    int selectedHotbarSlot = 0;

    void Start()
    {
        HotbarItemChanged();

        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        CheckForHotBarInput();
        inventoryParent.SetActive(isInventoryOpened);

        // Move item
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
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInventoryOpened = true;
            }
        }
    }

    private void CheckForHotBarInput ()
        {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
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
        for (int i = 0; i < handParent.childCount; i++)
        {
            handParent.GetChild(i).gameObject.SetActive(false);
        }
        foreach(GameObject slot in hotbarSlots)
        {
            Vector3 scale;

            if (slot == hotbarSlots[selectedHotbarSlot])
            {
                scale = new Vector3 (1.1f, 1.1f, 1.1f);

                if (slot.GetComponent<InventorySlot>().heldItem != null)
                {
                    for (int i = 0; i < handParent.childCount; i++)
                    {
                        if (handParent.GetChild(i).GetComponent<ItemHand>().itemScriptableObject == hotbarSlots[selectedHotbarSlot].GetComponent<InventorySlot>().heldItem.GetComponent<InventoryItem>().itemScriptableObject)
                        {
                            handParent.GetChild(i).gameObject.SetActive (true);
                        }
                    }
                }
            }
            else
            {
                scale = new Vector3(0.9f, 0.9f, 0.9f);
            }

            slot.transform.localScale = scale;
        }
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown called");

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            if (clickedObject == null)
            {
                Debug.LogError("Clicked object is null in OnPointerDown");
                return;
            }

            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            // There is an item in the slot - pick it up
            if (slot != null && slot.heldItem != null)
            {
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;
                Debug.Log("Item picked up from slot");
            }
            else
            {
                Debug.Log("No item in the clicked slot or slot is null");
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            //There isnt item in the slot - place item
            if (slot != null && slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            //There is item in the slot - switch items
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent == slot.heldItem.GetComponent<InventoryItem>().stackMax
                || slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject != draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(slot.heldItem);
                slot.heldItem.transform.SetParent(slot.transform.parent.parent.GetChild(2));

                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }

            //Fill stack
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent < slot.heldItem.GetComponent<InventoryItem>().stackMax
                && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject == draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                InventoryItem slotHeldItem = slot.heldItem.GetComponent<InventoryItem>();
                InventoryItem draggedItem = draggedObject.GetComponent<InventoryItem>();

                int itemsToFillStack = slotHeldItem.stackMax - slotHeldItem.stackCurrent;

                if (itemsToFillStack >= draggedItem.stackCurrent)
                {
                    slotHeldItem.stackCurrent += draggedItem.stackCurrent;
                    Destroy(draggedObject);
                }
                else
                {
                    slotHeldItem.stackCurrent += itemsToFillStack;
                    draggedItem.stackCurrent -= itemsToFillStack;
                    lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                }
            }
            //Return item to last slot
            else if (clickedObject.name != "DropItem")
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            //Drop item
            else
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);

                GameObject newItem = Instantiate(draggedObject.GetComponent<InventoryItem>().itemScriptableObject.prefab, position, new Quaternion());
                newItem.GetComponent<itemPickable>().itemScriptableObject = draggedObject.GetComponent<InventoryItem>().itemScriptableObject;

                lastItemSlot.GetComponent<InventorySlot>().heldItem = null;
                Destroy(draggedObject);
            }

            HotbarItemChanged();
            draggedObject = null;
        }
    }



    public void ItemPicked(GameObject pickedItem)
    {
        Debug.Log("ItemPicked called");

        // Vérifiez si l'objet est nul ou a été détruit
        if (pickedItem == null || !pickedItem.activeInHierarchy)
        {
            Debug.LogError("Picked item is null or has been destroyed!");
            return;
        }

        itemPickable pickableComponent = pickedItem.GetComponent<itemPickable>();
        if (pickableComponent == null)
        {
            Debug.LogError("Picked item does not have itemPickable component!");
            return;
        }

        ItemSO itemSO = pickableComponent.itemScriptableObject;
        if (itemSO == null)
        {
            Debug.LogError("Picked item does not have a valid ItemSO!");
            return;
        }

        // Cherche un slot vide
        GameObject emptySlot = null;
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            if (slot != null && slot.heldItem == null)
            {
                emptySlot = slots[i];
                break;
            }
        }

        if (emptySlot != null)
        {

            // Instancie un nouvel objet et le configure
            GameObject newItem = Instantiate(itemPrefab);
            InventoryItem inventoryItemComponent = newItem.GetComponent<InventoryItem>();
            if (inventoryItemComponent != null)
            {
                inventoryItemComponent.itemScriptableObject = itemSO;
                newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
                emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
                newItem.transform.localScale = new Vector3(1, 1, 1);

                newItem.GetComponent<InventoryItem>().stackCurrent = 1;
            
                Destroy(pickedItem);
                Debug.Log("Item placed in inventory slot");
            }
            else
            {
                Debug.LogError("New item does not have InventoryItem component!");
            }
        }
        else
        {
            Debug.LogError("No empty slot available in inventory!");
        }
    }




}