using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftingSlots : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    private ItemSO item; // Utiliser ItemSO directement
    public ItemSO Item => item;
    public GameObject heldItem;

    public delegate void ItemClicked(ItemSO item);
    public event ItemClicked OnItemClicked;

    public void SetItem(ItemSO newItem)
    {
        item = newItem;
        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            Debug.Log("Icon set to: " + (item != null ? item.icon.name : "null"));
        }
        else
        {
            Debug.LogError("Icon Image component is not assigned!");
        }
    }


    public void SetHeldItem(GameObject newItem)
    {
        heldItem = newItem;
        if (heldItem != null)
        {
            InventoryItem inventoryItem = heldItem.GetComponent<InventoryItem>();
            if (inventoryItem != null)
            {
                SetItem(inventoryItem.itemScriptableObject); // Assurez-vous que c'est de type ItemSO
            }
            heldItem.transform.SetParent(transform);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localScale = Vector3.one;
        }
        else
        {
            SetItem(null);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item != null && OnItemClicked != null)
        {
            OnItemClicked.Invoke(item);
        }
    }
}
