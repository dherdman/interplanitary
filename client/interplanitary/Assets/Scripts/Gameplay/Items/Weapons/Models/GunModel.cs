using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunModel : ItemModel
{
    [SerializeField]
    Transform _muzzle;
    public Transform Muzzle
    {
        get
        {
            return _muzzle;
        }
    }
}
