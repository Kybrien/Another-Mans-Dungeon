using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        public static bool IsWeapon(GameObject obj )
        {
            if(obj.transform.root.gameObject.GetComponent<Weapon>() != null)
            {
                return true;
            }
            else 
            {
                return false; 
            }
        }
    }
}
