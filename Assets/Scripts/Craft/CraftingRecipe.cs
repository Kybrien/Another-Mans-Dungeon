using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemSO> ingredients; // Liste des ingrédients nécessaires
    public ItemSO result; // Résultat du craft
}
