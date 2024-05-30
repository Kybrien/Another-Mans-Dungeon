using UnityEngine;
using TMPro;

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
        // Trouver tous les colliders dans la portée de ramassage
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);
        bool weaponNearby = false;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Weapon"))
            {
                weaponNearby = true;
                break;
            }
        }

        // Afficher ou masquer le message en fonction de la proximité d'une arme
        //pickupMessageText.gameObject.SetActive(weaponNearby);
    }

    void AttemptPickup()
    {
        // Trouver tous les colliders dans la portée de ramassage
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Weapon"))
            {
                PickupWeapon(collider.gameObject);
                break;
            }
        }
    }

    void PickupWeapon(GameObject weapon)
    {
        //faire l'anim de pick up
        animator.SetTrigger("Pickup");

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
        animator.SetTrigger("Idle");
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
        // Déclencher l'animation de récupération
        /*if (animator != null)
        {
            animator.SetTrigger("Pickup"); // Utiliser le trigger pour jouer l'animation de récupération
        }*/

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