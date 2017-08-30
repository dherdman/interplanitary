using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquippedItems
{
    [SerializeField]
    private List<Weapon> Weapons;
    int SelectedWeaponIndex = 0;
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

    public bool EquipItem(Item itm)
    {
        System.Type itemType = itm.GetType();

        if(itemType.IsSubclassOf(typeof(Weapon)))
        {
            return EquipWeapon((Weapon)itm);
        }
        else
        {
            return false; // base case, if unknown item type if cannot be equipped
        }
    }

    bool EquipWeapon(Weapon wpn)
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

        int startIndex = SelectedWeaponIndex;

        do
        {
            SelectedWeaponIndex += step;

            if (SelectedWeaponIndex < 0)
            {
                SelectedWeaponIndex = Weapons.Count - 1;
            }
            else if (SelectedWeaponIndex > Weapons.Count - 1)
            {
                SelectedWeaponIndex = 0;
            }

            if(SelectedWeaponIndex == startIndex)
            {
                break; // stop if we go in a full circle
            }
        } while (Weapons[SelectedWeaponIndex] == null);

        return Weapons[SelectedWeaponIndex];
    }
}
