using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    protected override void OnStart()
    {
        base.OnStart();

        GameManager.instance.RegisterPlayer(this);
    }
}
