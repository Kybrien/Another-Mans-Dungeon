using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private InventoryManager inventoryManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed");
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 10))
            {
                Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
                itemPickable item = hitInfo.collider.gameObject.GetComponent<itemPickable>();

                if (item != null)
                {
                    Debug.Log("Item found: " + item.name);
                    inventoryManager.ItemPicked(hitInfo.collider.gameObject);
                }
                else
                {
                    Debug.Log("No itemPickable component found on hit object");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything");
            }
        }
    }
}
