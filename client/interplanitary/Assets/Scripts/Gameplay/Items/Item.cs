using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour, IInteractable
{
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
