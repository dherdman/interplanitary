using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// !!! TODO implement damagable interface
[RequireComponent(typeof(Inventory)), RequireComponent(typeof(GenericCharacterController))]
public class Character : MonoBehaviour
{
    [SerializeField]
    Transform WeaponHold;

    [SerializeField]
    Item EquippedWeaponPrefab;
    public Item EquippedItem
    {
        get; private set;
    }

    GenericCharacterController CharacterControllerInstance;

    Inventory CharacterInventory;

    public bool HasFreeSlotInInventory
    {
        get
        {
            return CharacterInventory.HasFreeSlot;
        }
    }

    void Start()
    {
        CharacterControllerInstance = GetComponent<PlayerController>();
        CharacterInventory = GetComponent<Inventory>();

        if (EquippedWeaponPrefab)
        {
            EquipItem(EquippedWeaponPrefab);
        }
    }

    /// <summary>
    /// Toggles this player's controller enabled/disabled
    /// </summary>
    public void ToggleControl()
    {
        if (CharacterControllerInstance != null)
        {
            if (CharacterControllerInstance.enabled)
            {
                CharacterControllerInstance.DisableSelf();
            }
            else
            {
                CharacterControllerInstance.enabled = true;
            }
        }
    }

    // !!!!! TODO (also in player controller) should be animation based, this is temp
    public void AimAt(Vector3 aimTarget)
    {
        // only guns can be aimed, melee does not change based on mouse position
        if (EquippedItem != null && EquippedItem.GetType() == typeof(Gun))
        {
            WeaponHold.LookAt(aimTarget);
            WeaponHold.localRotation = Quaternion.Euler(WeaponHold.localRotation.eulerAngles.x, WeaponHold.localRotation.eulerAngles.y, 0f);
        }
    }

    public void UsePrimary()
    {
        if (EquippedItem != null)
        {
            EquippedItem.UsePrimary();
        }
    }

    public void EquipItem(Item toBeEquipped)
    {
        if(EquippedItem != null)
        {
            EquippedItem.DestroySelf();
        }

        EquippedItem = Instantiate(toBeEquipped, WeaponHold.position, WeaponHold.rotation).GetComponent<Item>();

        // disable item interaction hitbox 
        Collider c = EquippedItem.GetComponent<Collider>();
        if(c != null)
        {
            EquippedItem.GetComponent<Collider>().enabled = false; 
        }

        EquippedItem.transform.parent = WeaponHold;
    }

    public void EquipFromInventory ()
    {
        PickupItem(EquippedItem);
        EquipItem(CharacterInventory.GetItem());
    }

    public bool PickupItem(Item item)
    {
        return CharacterInventory.AddItem(item);
    }
}
