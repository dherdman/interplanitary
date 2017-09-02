using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryPickupDestination
{
    none,
    backpack,
    equip
}

[System.Serializable]
public class Inventory
{
    [SerializeField]
    Transform InventoryContainer;

    [SerializeField]
    public List<Item> Backpack;
    [SerializeField]
    public EquippedItems Equipped;

    Character character;
    public void SetCharacter(Character c)
    {
        character = c;
    }

    public bool HasSlotForItem(Item item)
    {
        return Equipped.HasSlotForItem(item) || Backpack.Contains(null);
    }

    public InventoryPickupDestination AddItem(Item item)
    {
        InventoryPickupDestination addState = InventoryPickupDestination.none;

        int slot = Equipped.EquipItem(item);

        if (slot >= 0)
        {
            addState = InventoryPickupDestination.equip;

            if(character.IsPlayer)
            {
                UpdateInventoryItemUI(InventoryPickupDestination.equip, slot);
            }
        }
        else 
        {
            slot = FillFirstOpenSlot(Backpack, item);

            if(slot >= 0)
            {
                addState = InventoryPickupDestination.backpack;

                if (character.IsPlayer)
                {
                    UpdateInventoryItemUI(InventoryPickupDestination.backpack, slot);
                }
            }
        }

        if (addState != InventoryPickupDestination.none)
        {
            // !!! TODO Will need revision when full equiped system is in place
            item.gameObject.SetActive(false); // item "dissapears" until in use
            item.GetComponent<Collider>().enabled = false; // Disable interaction trigger

            item.transform.parent = InventoryContainer;
            item.transform.localPosition = Vector3.zero; // !!! TODO this might not matter. Might be more relevent when re-enabling items
        }

        return addState;
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
        else if(pickupType == InventoryPickupDestination.equip)
        {
            if(invScreen != null)
            {
                invScreen.UpdateEquippedWeaponSlot(slot);
            }
            GameManager.instance.UpdateHudSlot(slot);
        }
    }

    // !!! TODO rework how backpack is stored. Slots should have no gaps
    public static int FillFirstOpenSlot<T>(List<T> list, T itm)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list[i] = itm;
                return i;
            }
        }
        return -1;
    }
}
