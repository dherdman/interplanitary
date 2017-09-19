using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquippedItems
{
    [SerializeField]
    public List<Weapon> Weapons;
    int _selectedWeaponIndex = 0;
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

    public bool HasSlotForItem(Item itm)
    {
        if (itm is Weapon)
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
    public int EquipItem(Item itm, params object[] parameters)
    {
        if (itm is Weapon)
        {
            int equipSlot = parameters.Length > 0 ? parameters[0] != null ? (int)parameters[0] : -1 : -1;
            return EquipWeapon((Weapon)itm, equipSlot);
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

    int EquipWeapon(Weapon wpn, int equipSlot)
    {
        if (equipSlot < 0 || equipSlot >= Weapons.Count) // if requested slot is out of bounds
        {
            equipSlot = Weapons.IndexOf(null);
        }

        if(equipSlot >= 0 && equipSlot < Weapons.Count)
        {
            Weapons[equipSlot] = wpn;
        }

        return equipSlot;
    }

    public Weapon CycleWeapon(bool shouldGetPrev = false)
    {
        int step = 1;
        if (shouldGetPrev)
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

            if (_selectedWeaponIndex == startIndex)
            {
                break; // stop if we go in a full circle
            }
        } while (Weapons[_selectedWeaponIndex] == null);

        return Weapons[_selectedWeaponIndex];
    }
}
