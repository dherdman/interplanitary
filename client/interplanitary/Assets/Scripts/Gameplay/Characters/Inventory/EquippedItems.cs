using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquippedItems
{
    [SerializeField]
    public List<Weapon> Weapons;
    int _selectedWeaponIndex = -1;
    public int SelectedWeaponIndex
    {
        get
        {
            return _selectedWeaponIndex;
        }
    }
    // !!! TODO will contain armor and such in the future

    public int WeaponCount
    {
        get
        {
            return Weapons == null ? 0 : Weapons.Count;
        }
    }

    public bool HasEmptyWeaponSlot
    {
        get
        {
            return Weapons.Contains(null);
        }
    }

    public bool HasSlotForItem (Item itm)
    {
        System.Type itemType = itm.GetType();

        if(itemType.IsSubclassOf(typeof(Weapon)))
        {
            return HasEmptyWeaponSlot;
        }
        else
        {
            return false; // base case, if unknown item type it cannot be equipped
        }
    }

    /// <summary>
    /// Returns -1 if unable to equip, else returns the slot number the item was equipped to
    /// </summary>
    public int EquipItem(Item itm)
    {
        System.Type itemType = itm.GetType();

        if(itemType.IsSubclassOf(typeof(Weapon)))
        {
            return EquipWeapon((Weapon)itm);
        }
        else
        {
            return -1; // base case, if unknown item type if cannot be equipped
        }
    }

    public bool IsWeaponSlotSelected(int i)
    {
        return i == SelectedWeaponIndex;
    }

    int EquipWeapon(Weapon wpn)
    {
        return Inventory.FillFirstOpenSlot(Weapons, wpn);
    }

    public Weapon CycleWeapon(bool shouldGetPrev = false)
    {
        int step = 1;
        if(shouldGetPrev)
        {
            step = -1; 
        }

        int startIndex = _selectedWeaponIndex;

        do
        {
            _selectedWeaponIndex += step;

            if (_selectedWeaponIndex < 0)
            {
                _selectedWeaponIndex = Weapons.Count - 1;
            }
            else if (_selectedWeaponIndex > Weapons.Count - 1)
            {
                _selectedWeaponIndex = 0;
            }

            if(_selectedWeaponIndex == startIndex)
            {
                break; // stop if we go in a full circle
            }
        } while (Weapons[_selectedWeaponIndex] == null);

        return Weapons[_selectedWeaponIndex];
    }
}
