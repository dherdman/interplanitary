using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// !!! TODO implement damagable interface
[RequireComponent(typeof(GenericCharacterController))]
public abstract class Character : MonoBehaviour
{
    [SerializeField]
    Transform WeaponHold;
    [SerializeField]
    Transform EquippedItemsContainer;
    [SerializeField]
    Transform InventoryContainer;

    [SerializeField]
    Item InitiallyEquippedWeaponPrefab;
    public Item SelectedItem
    {
        get; private set;
    }

    // !!! TODO if online introduced this won't behave as expected
    public bool IsPlayer
    {
        get; private set;
    }

    GenericCharacterController CharacterControllerInstance;

    [SerializeField]
    public Inventory CharacterInventory;

    public bool HasSlotForItemInInventory(Item itm)
    {
        return CharacterInventory.HasSlotForItem(itm);
    }

    void Start()
    {
        CharacterInventory.Init(this);

        CharacterControllerInstance = GetComponent<GenericCharacterController>();

        IsPlayer = this.GetType() == typeof(Player);

        if (InitiallyEquippedWeaponPrefab)
        {
            PickupItem(Instantiate(InitiallyEquippedWeaponPrefab));
            CycleWeapon(true);
        }

        OnStart();
    }
    protected virtual void OnStart()
    {

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
        // only guns can be aimed
        if (SelectedItem != null && SelectedItem.GetType() == typeof(Gun))
        {
            WeaponHold.LookAt(aimTarget);
            WeaponHold.localRotation = Quaternion.Euler(WeaponHold.localRotation.eulerAngles.x, 0f, 0f);
        }
    }

    public void UsePrimary()
    {
        if (SelectedItem != null)
        {
            SelectedItem.UsePrimary();
        }
    }

    public void EquipItem(Item toBeEquipped)
    {
        if (SelectedItem != null)
        {
            SelectedItem.gameObject.SetActive(false);
            SelectedItem.transform.parent = EquippedItemsContainer;
        }

        SelectedItem = toBeEquipped;
        SelectedItem.transform.parent = WeaponHold;
        SelectedItem.transform.localPosition = Vector3.zero;
        SelectedItem.transform.localRotation = Quaternion.identity;
        toBeEquipped.gameObject.SetActive(true);

        // disable item interaction hitbox 
        Collider c = SelectedItem.GetComponent<Collider>();
        if (c != null)
        {
            c.enabled = false;
        }
    }

    public bool DropSelectedItem ()
    {
        if(SelectedItem != null)
        {
            // !!! TODO implement properly
            SelectedItem.transform.parent = null;
            SelectedItem.transform.position = Vector3.zero; // !!! TODO drop on ground
            SelectedItem.transform.rotation = Quaternion.identity;

            // enable gameobject and item interact collider
            SelectedItem.gameObject.SetActive(true);
            Collider c = SelectedItem.GetComponent<Collider>();
            if (c != null)
            {
                c.enabled = true;
            }

            SelectedItem = null; // remove reference to item

            return true;
        }

        return false;
    }

    public bool PutSelectedItemInBackpack()
    {
        // !!! TODO should call a generalized fn for any equipped item to backpack

        // if SelectedItem is non-null, try add to inventory. 
        // If succeeds, place SelectedItem gameobject in inventory container & dereference
        if(SelectedItem != null && CharacterInventory.AddItem(SelectedItem, false).Destination != InventoryPickupDestination.none)
        {
            // disable gameobject, no need to worry about collider b/c is already disabled if selected
            SelectedItem.gameObject.SetActive(false);
            SelectedItem.transform.parent = InventoryContainer;
            SelectedItem = null; // remove reference to item

            return true;
        }
        return false;
    }

    public void CycleWeapon(bool shouldGetPrev)
    {
        int initiallySelected = CharacterInventory.Equipped.SelectedWeaponIndex;
        Weapon w = CharacterInventory.Equipped.CycleWeapon(shouldGetPrev);

        // If a new weapon has been selected
        if (CharacterInventory.Equipped.SelectedWeaponIndex != initiallySelected)
        {
            // equip the new weapon
            EquipItem(w);

            // Update the hud. Un-Select initiallySelected, Select SelectedWeaponIndex
            if (IsPlayer && GameManager.instance.GameHud != null)
            {
                GameManager.instance.GameHud.UpdateWeaponSlot(initiallySelected, false);
                GameManager.instance.GameHud.UpdateWeaponSlot(CharacterInventory.Equipped.SelectedWeaponIndex, true, w);

                InventoryScreen invScreen = (InventoryScreen)UIManager.instance.GetScreenInstance(ScreenName.Inventory);
                if (invScreen != null)
                {
                    invScreen.UpdateEquippedWeaponSlot(initiallySelected);
                    invScreen.UpdateEquippedWeaponSlot(CharacterInventory.Equipped.SelectedWeaponIndex);
                }
            }
        }

    }

    public bool PickupItem(Item item)
    {
        InventoryPickupState pickupState = CharacterInventory.AddItem(item, true);

        if(pickupState.Destination != InventoryPickupDestination.none)
        {
            item.gameObject.SetActive(false); // item "dissapears" until in use
            item.GetComponent<Collider>().enabled = false; // Disable interaction trigger
        }

        if(pickupState.Destination == InventoryPickupDestination.backpack)
        {
            item.transform.parent = InventoryContainer;
            item.transform.localPosition = Vector3.zero; // !!! TODO this might not matter. Might be more relevent when re-enabling items
        }
        else if(pickupState.Destination == InventoryPickupDestination.equipped || pickupState.Destination == InventoryPickupDestination.selected)
        {
            item.transform.parent = EquippedItemsContainer;
            item.transform.localPosition = Vector3.zero; // !!! TODO this might not matter. Might be more relevent when re-enabling items

            // If new item replaced selected, attempt to put current item in backpack, else drop it
            if(pickupState.Destination == InventoryPickupDestination.selected)
            {
                if(SelectedItem != null)
                {
                    // if was not able to add to backpack, drop selected item
                    if (!PutSelectedItemInBackpack()) 
                    {
                        DropSelectedItem();
                    }
                }
                EquipItem(item);
            }

            if (GameManager.instance.GameHud != null && item is Weapon)
            {
                GameManager.instance.GameHud.UpdateWeaponSlot(
                    pickupState.Slot, 
                    pickupState.Slot == CharacterInventory.Equipped.SelectedWeaponIndex, 
                    (Weapon)item);
            }
        }
                
        return pickupState.Destination != InventoryPickupDestination.none;
    }
}
