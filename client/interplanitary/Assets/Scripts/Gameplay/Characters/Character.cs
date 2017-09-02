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
        CharacterInventory.SetCharacter(this);

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
            SelectedItem.GetComponent<Collider>().enabled = false;
        }

        SelectedItem.transform.parent = WeaponHold;
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
        return CharacterInventory.AddItem(item) != InventoryPickupDestination.none;
    }
}
