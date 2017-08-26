using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item
{
    [Header("Weapon Properties")]
    [SerializeField]
    protected float Damage;

    public abstract void Fire();
    public virtual void Reload()
    {
        // Does not apply for Melee weapons
    }
}
