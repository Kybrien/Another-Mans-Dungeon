using UnityEngine;

[CreateAssetMenu(fileName = "New CraftableItem", menuName = "Crafting/Craftable Item")]
public class CraftableItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
}
