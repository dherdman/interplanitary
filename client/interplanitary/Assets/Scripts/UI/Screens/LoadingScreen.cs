using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : UIScreen
{
    public override ScreenName screenName
    {
        get
        {
            return ScreenName.Loading;
        }
    }

    public override IEnumerator Init(params object[] parameters)
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
