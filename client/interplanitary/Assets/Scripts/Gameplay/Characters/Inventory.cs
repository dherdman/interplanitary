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
}
