using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypes
{
    weapon,
    armor,
    consumable
}

[Serializable]
public class Item : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject _itemModel;
    public GameObject ItemModel
    {
        get
        {
            return _itemModel;
        }
    }
    GameObject itemInstance;


    bool disabled;
    [SerializeField]
    ItemTypes _itemType;
    public ItemTypes ItemType
    {
        get
        {
            return _itemType;
        }
    }

    void OnEnable()
    {
        itemInstance = Instantiate(ItemModel, transform);
        disabled = false;
    }

    public bool CanInteract(Character character)
    {
        return character.HasFreeSlotInInventory;
    }

    public void Interact(Character character)
    {
        if(!disabled && character.PickupItem(this))
        {
            disabled = true;
            DestroySelf();
        }
    }

    public void DestroySelf(bool fullDestroy = false)
    {
        if(fullDestroy)
        {
            Destroy(this);
        }
        else
        {
            Destroy(itemInstance);
        }
    }

    public virtual void UsePrimary()
    {

    }
}
