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

public class Item : MonoBehaviour, IInteractable
{
    public bool CanInteract(Player playerInstance)
    {
        throw new NotImplementedException();
    }

    public void Interact(Player playerInstance)
    {
        playerInstance.PickupItem(this);
    }
}
