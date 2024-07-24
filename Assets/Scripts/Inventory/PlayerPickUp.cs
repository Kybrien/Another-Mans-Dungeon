using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private InventoryManager inventoryManager;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (cam == null)
            {
                Debug.LogError("Camera reference is missing!");
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 3))
            {
                itemPickable item = hitInfo.collider.gameObject.GetComponent<itemPickable>();

                if (item == null)
                {
                    Debug.Log("itemPickable component is missing on hit object");
                }
                else
                {
                    if (inventoryManager == null)
                    {
                        Debug.LogError("InventoryManager reference is missing!");
                        return;
                    }

                    inventoryManager.ItemPicked(hitInfo.collider.gameObject);
                }
            }

        }
    }

}
