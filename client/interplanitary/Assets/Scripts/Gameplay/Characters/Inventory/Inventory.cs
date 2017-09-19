using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryPickupDestination
{
    none,
    backpack,
    equipped,
    selected
}

public class InventoryPickupState
{
    public InventoryPickupDestination Destination { get; private set; }
    public int Slot { get; private set; }

    public InventoryPickupState() : this(-1, InventoryPickupDestination.none)
    {
    }
    public InventoryPickupState(int _slot, InventoryPickupDestination _dest)
    {
        Slot = _slot;
        Destination = _dest;
    }
}

[System.Serializable]
public class Inventory
{
    [SerializeField]
    public List<Item> Backpack;
    int nextBackpackSlot = 0;

    [SerializeField]
    public EquippedItems Equipped;

    public Character ParentCharacter
    {
        get; private set;
    }

    public void Init(Character c)
    {
        ParentCharacter = c;
    }

    public bool HasSlotForItem(Item item)
    {
        return Equipped.HasSlotForItem(item) || nextBackpackSlot < Backpack.Count;
    }

    public InventoryPickupState AddItem(Item item, bool attemptEquip)
    {
        InventoryPickupDestination destination = InventoryPickupDestination.none;

        int slot = -1;

        if(attemptEquip)
        {
            slot = Equipped.EquipItem(item);
        }
        if (slot >= 0)
        {
            // !!! TODO will need to generalize for different item types
            destination = slot == Equipped.SelectedWeaponIndex ? InventoryPickupDestination.selected : InventoryPickupDestination.equipped;

            if (ParentCharacter.IsPlayer)
            {
                UpdateInventoryItemUI(InventoryPickupDestination.equipped, slot);
            }
        }
        else 
        {
            slot = AddToBackpack(item);

            if(slot >= 0)
            {
                destination = InventoryPickupDestination.backpack;

                if (ParentCharacter.IsPlayer)
                {
                    UpdateInventoryItemUI(InventoryPickupDestination.backpack, slot);
                }
            }
        }

        return new InventoryPickupState(slot, destination);
    }

    void UpdateInventoryItemUI(InventoryPickupDestination pickupType, int slot)
    {
        InventoryScreen invScreen = (InventoryScreen)UIManager.instance.GetScreenInstance(ScreenName.Inventory);

        if(pickupType == InventoryPickupDestination.backpack)
        {
            if(invScreen != null)
            {
                invScreen.UpdateBackpackSlot(slot);
            }
        }
        else if(pickupType == InventoryPickupDestination.equipped)
        {
            if(invScreen != null)
            {
                invScreen.UpdateEquippedWeaponSlot(slot);
            }
            GameManager.instance.UpdateHudSlot(slot);
        }
    }

    int AddToBackpack(Item itm)
    {
        if (nextBackpackSlot < Backpack.Count)
        {
            Backpack[nextBackpackSlot] = itm;
            return nextBackpackSlot++;
        }
        return -1;
    }
}
