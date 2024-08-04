/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public List<CraftingCombination> craftingCombinations;
    public Transform itemsParent;
    public GameObject itemSlotPrefab;
    public CraftingSlot resultSlot;  // Slot pour afficher le résultat
    public Button craftButton;

    public ItemSO itemSOExample; // Déclaration de l'élément d'exemple

    private ItemSO selectedItem1;
    private ItemSO selectedItem2;

    private void Start()
    {
        if (itemSlotPrefab == null || itemsParent == null || resultSlot == null || craftButton == null)
        {
            Debug.LogError("One or more required components are not assigned!");
            return;
        }

        if (craftingCombinations == null || craftingCombinations.Count == 0)
        {
            Debug.LogError("Crafting combinations list is empty or not assigned!");
            return;
        }

        PopulateCraftingUI();
        craftButton.onClick.AddListener(OnCraftButtonClicked);
    }

    void PopulateCraftingUI()
    {
        foreach (ItemSO item in GetCraftableItems())
        {
            GameObject newItemSlot = Instantiate(itemSlotPrefab, itemsParent);
            CraftingSlot slot = newItemSlot.GetComponent<CraftingSlot>();
            if (slot != null)
            {
                slot.SetItem(item);
                slot.OnItemClicked += OnItemSlotClicked;
                Debug.Log("Populated slot with item: " + item.name);
            }
            else
            {
                Debug.LogError("CraftingSlot component not found on the instantiated prefab!");
            }
        }
    }


    List<ItemSO> GetCraftableItems()
    {
        List<ItemSO> items = new List<ItemSO>();
        if (itemSOExample != null)
        {
            items.Add(itemSOExample);
        }
        return items;
    }

    void OnItemSlotClicked(ItemSO item)
    {
        Debug.Log("Item slot clicked: " + item.name);
        if (selectedItem1 == null)
        {
            selectedItem1 = item;
            Debug.Log("Selected Item 1: " + selectedItem1.name);
        }
        else if (selectedItem2 == null)
        {
            selectedItem2 = item;
            Debug.Log("Selected Item 2: " + selectedItem2.name);
        }

        if (selectedItem1 != null && selectedItem2 != null)
        {
            CheckCraftingCombination();
        }
    }

    void CheckCraftingCombination()
    {
        bool combinationFound = false;
        Debug.Log($"Selected Item 1: {selectedItem1?.name}");
        Debug.Log($"Selected Item 2: {selectedItem2?.name}");

        foreach (CraftingCombination combination in craftingCombinations)
        {
            Debug.Log($"Checking combination: {combination.item1?.name} + {combination.item2?.name}");

            if (combination.item1 == null || combination.item2 == null)
            {
                Debug.LogWarning("One or both items in the combination are null.");
                continue;
            }

            if ((combination.item1 == selectedItem1 && combination.item2 == selectedItem2) ||
                (combination.item1 == selectedItem2 && combination.item2 == selectedItem1))
            {
                Debug.Log($"Combination found: {combination.item1?.name} + {combination.item2?.name}");
                SetCraftingResult(combination.result);
                combinationFound = true;
                break;
            }
        }
        if (!combinationFound)
        {
            Debug.Log("No valid crafting combination found.");
            SetCraftingResult(null);
        }
    }





    void SetCraftingResult(ItemSO resultItem)
    {
        if (resultSlot != null)
        {
            resultSlot.SetItem(resultItem);
            if (resultItem != null)
            {
                Debug.Log("Result item set to: " + resultItem.name);
            }
            else
            {
                Debug.Log("Result item is null.");
            }
        }
        else
        {
            Debug.LogError("resultSlot is not assigned!");
        }
    }


    public void OnCraftButtonClicked()
    {
        Debug.Log("Craft button clicked.");
        if (resultSlot != null)
        {
            if (resultSlot.Item != null)
            {
                Debug.Log("Crafting completed: " + resultSlot.Item.name);
                selectedItem1 = null;
                selectedItem2 = null;
                SetCraftingResult(null);
            }
            else
            {
                Debug.LogError("No crafting result available.");
            }
        }
        else
        {
            Debug.LogError("resultSlot is not assigned!");
        }
    }


}
*/