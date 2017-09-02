using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField]
    Sprite _icon;
    public Sprite Icon
    {
        get
        {
            return _icon;
        }
    }

    [SerializeField]
    string _itemName;
    public string ItemName
    {
        get
        {
            return _itemName;
        }
    }

    public bool CanInteract(Character character)
    {
        return character.HasSlotForItemInInventory(this);
    }

    public void Interact(Character character)
    {
        character.PickupItem(this);
    }

    public void DestroySelf()
    {
        Destroy(this);
    }

    public virtual void UsePrimary()
    {

    }
}
