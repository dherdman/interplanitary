using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    int InventorySlots;

    List<Item> Items;

    void Start()
    {
        Items = new List<Item>(InventorySlots);
    }

    public bool HasFreeSlot
    {
        get
        {
            return Items.Count < InventorySlots;
        }
    }

    public bool AddItem(Item item)
    {
        if (HasFreeSlot)
        {
            Items.Add(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public Item GetItem()
    {
        Item i = Items[0];
        Items.RemoveAt(0);
        return i;
    }
}
