using UnityEngine;

namespace ItemNamespace
{
    public class Item : MonoBehaviour
    {
        public enum ItemType
        {
            Potion,
            Other
        }

        public string itemName;
        public ItemType itemType;

        public static bool IsItem(GameObject obj)
        {
            return obj.transform.root.gameObject.GetComponent<Item>() != null;
        }
    }
}
