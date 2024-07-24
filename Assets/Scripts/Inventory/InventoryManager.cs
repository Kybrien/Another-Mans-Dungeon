using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public bool isStorageOpened;

    [SerializeField] GameObject[] hotbarSlots = new GameObject[4];
    [SerializeField] GameObject[] slots = new GameObject[20];
    [SerializeField] GameObject inventoryParent;

    [SerializeField] GameObject itemPrefab;
    [SerializeField] Camera cam;

    GameObject draggedObject;
    GameObject lastItemSlot;
    bool isInventoryOpened;
    int selectedHotbarSlot = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Vérification des références
        if (inventoryParent == null)
            Debug.LogError("Inventory parent is not assigned!");

        if (itemPrefab == null)
            Debug.LogError("Item prefab is not assigned!");

        if (cam == null)
            Debug.LogError("Camera is not assigned!");
    }

    void Update()
    {
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
        Debug.Log("OnPointerUp called");

        if (draggedObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

            // Check if we clicked outside of inventory slots to drop the item
            if (clickedObject == null || clickedObject.GetComponent<InventorySlot>() == null)
            {
                if (cam == null)
                {
                    Debug.LogError("Camera reference is missing!");
                    return;
                }

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);

                InventoryItem draggedInventoryItem = draggedObject.GetComponent<InventoryItem>();
                if (draggedInventoryItem != null && draggedInventoryItem.itemScriptableObject != null)
                {
                    GameObject newItem = Instantiate(draggedInventoryItem.itemScriptableObject.prefab, position, Quaternion.identity);
                    itemPickable itemPickableComponent = newItem.GetComponent<itemPickable>();
                    if (itemPickableComponent != null)
                    {
                        itemPickableComponent.itemScriptableObject = draggedInventoryItem.itemScriptableObject;

                        if (lastItemSlot != null)
                        {
                            InventorySlot lastSlot = lastItemSlot.GetComponent<InventorySlot>();
                            if (lastSlot != null)
                            {
                                lastSlot.heldItem = null;
                                Debug.Log("Item dropped");
                            }
                            else
                            {
                                Debug.LogError("Last item slot does not have an InventorySlot component!");
                            }
                        }
                        else
                        {
                            Debug.LogError("Last item slot is null!");
                        }

                        Destroy(draggedObject);
                    }
                    else
                    {
                        Debug.LogError("New item does not have itemPickable component!");
                    }
                }
                else
                {
                    Debug.LogError("Dragged object does not have InventoryItem component or its itemScriptableObject is null!");
                }

                draggedObject = null;
                return;
            }

            // Handle case when clickedObject is an InventorySlot
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();
            if (slot == null)
            {
                Debug.LogError("Clicked object does not have an InventorySlot component in OnPointerUp");

                // Attempt to return the dragged object to its last slot
                if (lastItemSlot != null)
                {
                    InventorySlot lastSlot = lastItemSlot.GetComponent<InventorySlot>();
                    if (lastSlot != null)
                    {
                        lastSlot.SetHeldItem(draggedObject);
                        draggedObject.transform.SetParent(lastSlot.transform.parent.parent.GetChild(2));
                        Debug.Log("Item returned to last slot");
                    }
                    else
                    {
                        Debug.LogError("Last item slot does not have an InventorySlot component!");
                    }
                }
                else
                {
                    Debug.LogError("Last item slot is null!");
                }

                draggedObject = null;
                return;
            }

            if (slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
                Debug.Log("Item placed in slot");
            }
            else
            {
                if (lastItemSlot != null)
                {
                    InventorySlot lastSlot = lastItemSlot.GetComponent<InventorySlot>();
                    if (lastSlot != null)
                    {
                        lastSlot.SetHeldItem(draggedObject);
                        draggedObject.transform.SetParent(lastSlot.transform.parent.parent.GetChild(2));
                        Debug.Log("Item returned to last slot");
                    }
                    else
                    {
                        Debug.LogError("Last item slot does not have an InventorySlot component!");
                    }
                }
                else
                {
                    Debug.LogError("Last item slot is null!");
                }
            }

            draggedObject = null;
        }
        else
        {
            Debug.LogError("Conditions not met for OnPointerUp: draggedObject or eventData.pointerCurrentRaycast.gameObject is null or wrong mouse button");
        }
    }

    public void ItemPicked(GameObject pickedItem)
    {
        Debug.Log("ItemPicked called");

        if (pickedItem == null)
        {
            Debug.LogError("Picked item is null!");
            return;
        }

        GameObject emptySlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot.heldItem == null)
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
            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);
            Destroy(pickedItem);
            Debug.Log("Item placed in inventory slot");
        }
        else
        {
            Debug.LogError("No empty slot available in inventory!");
        }
    }
}


