using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsScreen : UIScreen
{
    public override ScreenName screenName
    {
        get
        {
            return ScreenName.Settings;
        }
    }

    public override IEnumerator Init()
    {
        yield return new WaitForSeconds(5);
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
