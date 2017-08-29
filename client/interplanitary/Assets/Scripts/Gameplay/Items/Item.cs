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
    ItemModel _itemModel;
    public ItemModel ItemModelInstance
    {
        get
        {
            return itemInstance;
        }
    }
    ItemModel itemInstance;


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
        itemInstance = Instantiate(_itemModel.gameObject, transform).GetComponent<ItemModel>();
        disabled = false;

        OnEnableEnd();
    }

    protected virtual void OnEnableEnd()
    {

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
            Destroy(itemInstance.gameObject);
        }
    }

    public virtual void UsePrimary()
    {

    }
}
