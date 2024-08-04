using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemSO> ingredients; // Liste des ingr�dients n�cessaires
    public ItemSO result; // R�sultat du craft
}
