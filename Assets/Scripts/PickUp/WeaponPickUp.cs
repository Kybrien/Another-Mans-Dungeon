using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WeaponPickup : MonoBehaviour
{
    public float pickupRange = 2.0f; // Distance maximale pour ramasser une arme
    public Transform weaponHoldPosition; // Position où l'arme sera tenue
    public Animator animator; // Référence à l'Animator
    public TextMeshProUGUI pickupMessageText; // Référence à l'élément TextMeshPro de l'UI

    private GameObject weapon1; // Référence à la première arme
    private GameObject weapon2; // Référence à la deuxième arme
    private GameObject activeWeapon; // Référence à l'arme actuellement active
    private int activeWeaponIndex = 0; // Index de l'arme active (0 ou 1)
    private GameObject nearestPickup; // Référence à l'objet le plus proche

    private List<GameObject> collectedItems = new List<GameObject>(); // Liste des objets collectés (non-armes)

    void Update()
    {
        DisplayPickupMessage();

        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPickup();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
    }

    void DisplayPickupMessage()
    {
        nearestPickup = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Weapon") || collider.CompareTag("Potion") || collider.CompareTag("Armor") || collider.CompareTag("Other"))
            {
                nearestPickup = collider.gameObject;
                break;
            }
        }

        if (nearestPickup != null)
        {
            WeaponNamespace.Weapon weaponComponent = nearestPickup.GetComponent<WeaponNamespace.Weapon>();
            ItemNamespace.Item itemComponent = nearestPickup.GetComponent<ItemNamespace.Item>();
            if (weaponComponent != null)
            {
                pickupMessageText.text = $"F pour ramasser {weaponComponent.weaponName}";
            }
            else if (itemComponent != null)
            {
                pickupMessageText.text = $"F pour ramasser {itemComponent.itemName}";
            }
            pickupMessageText.gameObject.SetActive(true);
        }
        else
        {
            pickupMessageText.gameObject.SetActive(false);
        }
    }

    void AttemptPickup()
    {
        if (nearestPickup != null)
        {
            WeaponNamespace.Weapon weaponComponent = nearestPickup.GetComponent<WeaponNamespace.Weapon>();
            ItemNamespace.Item itemComponent = nearestPickup.GetComponent<ItemNamespace.Item>();

            if (weaponComponent != null)
            {
                PickupWeapon(nearestPickup);
            }
            else if (itemComponent != null)
            {
                PickupItem(nearestPickup);
            }
        }
    }

    void PickupWeapon(GameObject weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon is null.");
            return;
        }

        // Faire l'animation de ramassage
        if (animator != null)
        {
            animator.SetTrigger("Pickup");
        }
        else
        {
            Debug.LogError("Animator is not assigned.");
        }

        if (weapon1 == null)
        {
            weapon1 = weapon;
            PositionWeapon(weapon);
            activeWeapon = weapon1;
            activeWeapon.SetActive(true);
        }
        else if (weapon2 == null)
        {
            weapon2 = weapon;
            PositionWeapon(weapon);
            weapon2.SetActive(false);
        }
        else
        {
            // Si les deux emplacements sont occupés, remplacer l'arme active
            DropCurrentWeapon();
            PickupWeapon(weapon);
        }

        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }

        // Masquer le message de ramassage
        pickupMessageText.gameObject.SetActive(false);
    }

    void PickupItem(GameObject item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null.");
            return;
        }

        // Faire l'animation de ramassage
        if (animator != null)
        {
            animator.SetTrigger("Pickup");
        }
        else
        {
            Debug.LogError("Animator is not assigned.");
        }

        // Ajouter l'objet à la liste des objets collectés
        collectedItems.Add(item);
        item.SetActive(false);

        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }

        // Masquer le message de ramassage
        pickupMessageText.gameObject.SetActive(false);
    }

    void DropCurrentWeapon()
    {
        if (activeWeapon != null)
        {
            // Réactiver la physique de l'arme actuelle
            Rigidbody rb = activeWeapon.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true; // Réactiver les collisions
            }

            // Détacher l'arme de la main du personnage
            activeWeapon.transform.parent = null;

            if (activeWeapon == weapon1)
            {
                weapon1 = null;
            }
            else if (activeWeapon == weapon2)
            {
                weapon2 = null;
            }

            activeWeapon = null;
        }
    }

    void PositionWeapon(GameObject weapon)
    {
        // Désactiver la physique de l'arme pour qu'elle ne tombe pas
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false; // Désactiver les collisions pour éviter les interférences
        }

        // Positionner l'arme dans la position de la main du personnage
        weapon.transform.position = weaponHoldPosition.position;
        weapon.transform.rotation = weaponHoldPosition.rotation;
        weapon.transform.parent = weaponHoldPosition;
    }

    void SwitchWeapon(int weaponIndex)
    {
        if ((weaponIndex == 0 && weapon1 != null) || (weaponIndex == 1 && weapon2 != null))
        {
            if (activeWeapon != null)
            {
                activeWeapon.SetActive(false);
            }

            activeWeaponIndex = weaponIndex;

            if (activeWeaponIndex == 0)
            {
                activeWeapon = weapon1;
            }
            else
            {
                activeWeapon = weapon2;
            }

            activeWeapon.SetActive(true);
        }
    }
}
