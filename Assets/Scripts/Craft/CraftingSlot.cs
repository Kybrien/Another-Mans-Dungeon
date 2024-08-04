using UnityEngine;

public class CraftingSlot : MonoBehaviour
{
    public GameObject HeldItem;

    // Définir un délégué et un événement pour signaler les changements
    public delegate void SlotChanged();
    public event SlotChanged OnSlotChanged;

    public void SetHeldItem(GameObject item)
    {
        HeldItem = item;
        if (item != null)
        {
            HeldItem.transform.position = transform.position;
            HeldItem.transform.SetParent(transform);
        }
        // Déclencher l'événement lorsque le contenu du slot change
        OnSlotChanged?.Invoke();
    }

    public void ClearSlot()
    {
        if (HeldItem != null)
        {
            Destroy(HeldItem);
        }
        HeldItem = null;
        // Déclencher l'événement lorsque le contenu du slot change
        OnSlotChanged?.Invoke();
    }

    public bool IsEmpty()
    {
        return HeldItem == null;
    }
}
