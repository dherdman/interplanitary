using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHudScreen : UIScreen
{
    public override ScreenName screenName
    {
        get
        {
            return ScreenName.GameHud;
        }
    }

    public override IEnumerator Init()
    {
        yield return null;
    }

    protected override void OnExit()
    {
    }

    protected override void OnStart()
    {
    }

    protected override void OnUpdate()
    {
    }
}
