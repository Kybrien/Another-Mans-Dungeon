using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private List<CraftingRecipe> recipes; // Liste des recettes disponibles pour le craft
    [SerializeField] private List<CraftingSlot> craftingSlots; // Liste des slots de crafting
    [SerializeField] private GameObject resultSlot; // Slot de résultat


    private void Start()
    {
        foreach (CraftingSlot slot in craftingSlots)
        {
            slot.OnSlotChanged += CheckCraftingConditions;
        }
    }

    private void CheckCraftingConditions()
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            if (CanCraft(recipe))
            {
                Craft(recipe);
                return; // On s'arrête après avoir crafté une recette valide
            }
        }
    }

    private bool CanCraft(CraftingRecipe recipe)
    {
        Dictionary<ItemSO, int> ingredientCount = new Dictionary<ItemSO, int>();

        foreach (ItemSO ingredient in recipe.ingredients)
        {
            if (!ingredientCount.ContainsKey(ingredient))
            {
                ingredientCount[ingredient] = 0;
            }
            ingredientCount[ingredient]++;
        }

        foreach (ItemSO ingredient in ingredientCount.Keys)
        {
            int totalCount = 0;
            foreach (CraftingSlot slot in craftingSlots)
            {
                if (slot.HeldItem != null)
                {
                    InventoryItem item = slot.HeldItem.GetComponent<InventoryItem>();
                    if (item.itemScriptableObject == ingredient)
                    {
                        totalCount += item.stackCurrent;
                    }
                }
            }

            if (totalCount < ingredientCount[ingredient])
            {
                return false; // Pas assez d'ingrédients
            }
        }

        return true;
    }

    public void AttemptCraft()
    {
        CheckCraftingConditions();
    }

    private void Craft(CraftingRecipe recipe)
    {
        // Utiliser un dictionnaire pour suivre combien d'ingrédients nous avons encore besoin
        Dictionary<ItemSO, int> ingredientsToRemove = new Dictionary<ItemSO, int>();

        foreach (ItemSO ingredient in recipe.ingredients)
        {
            if (!ingredientsToRemove.ContainsKey(ingredient))
            {
                ingredientsToRemove[ingredient] = ingredient.stackMax;
            }
        }

        // Retirer les ingrédients des slots de crafting
        foreach (ItemSO ingredient in ingredientsToRemove.Keys)
        {
            int amountToRemove = ingredientsToRemove[ingredient];
            foreach (CraftingSlot slot in craftingSlots)
            {
                if (slot.HeldItem != null)
                {
                    InventoryItem item = slot.HeldItem.GetComponent<InventoryItem>();
                    if (item.itemScriptableObject == ingredient)
                    {
                        int amountToTake = Mathf.Min(item.stackCurrent, amountToRemove);
                        amountToRemove -= amountToTake;
                        item.stackCurrent -= amountToTake;
                        if (item.stackCurrent <= 0)
                        {
                            slot.SetHeldItem(null);
                        }

                        if (amountToRemove <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        // Ajouter le résultat au slot de résultat
        GameObject resultItem = Instantiate(inventoryManager.ItemPrefab);
        resultItem.GetComponent<InventoryItem>().itemScriptableObject = recipe.result;
        resultItem.GetComponent<InventoryItem>().stackCurrent = 1;

        if (resultSlot != null)
        {
            var resultSlotComponent = resultSlot.GetComponent<InventorySlot>();
            if (resultSlotComponent != null)
            {
                resultSlotComponent.SetHeldItem(resultItem);
                resultItem.transform.SetParent(resultSlot.transform);
                resultItem.transform.localScale = Vector3.one;
            }
        }
    }

}