using UnityEngine;

namespace WeaponNamespace
{
    public class Weapon : MonoBehaviour
    {
        public string weaponName;

        public static bool IsWeapon(GameObject obj)
        {
            return obj.transform.root.gameObject.GetComponent<Weapon>() != null;
        }
    }
}
