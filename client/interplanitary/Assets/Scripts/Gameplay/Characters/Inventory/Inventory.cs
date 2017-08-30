using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField]
    Transform InventoryContainer;

    [SerializeField]
    List<Item> ReserveItems;
    [SerializeField]
    EquippedItems Equipped;

    void Start()
    {
        Debug.LogError("[Inventory] Inventory container not given!");
    }

    public bool HasSlotForItem(Item item)
    {
        return Equipped.HasSlotForItem(item) || ReserveItems.Contains(null);
    }

    public bool AddItem(Item item)
    {
        if(Equipped.EquipItem(item) || FillFirstOpenSlot(ReserveItems, item))
        {
            // !!! TODO Will need revision when full equiped system is in place
            item.gameObject.SetActive(false); // item "dissapears" until in use
            item.GetComponent<Collider>().enabled = false; // Disable interaction trigger

            item.transform.parent = InventoryContainer;
            item.transform.localPosition = Vector3.zero; // !!! TODO this might not matter. Might be more relevent when re-enabling items

            return true;
        }
        return false;
    }

    public Weapon CycleWeapon(bool shouldGetPrev = false)
    {
        return Equipped.CycleWeapon(shouldGetPrev);
    }

    public static bool FillFirstOpenSlot<T>(List<T> list, T itm)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                list[i] = itm;
                return true;
            }
        }
        return false;
    }
}
