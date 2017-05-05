﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScreen : UIScreen
{
    public override ScreenName screenName
    {
        get
        {
            return ScreenName.MainMenu;
        }
    }

    public override IEnumerator Init()
    {
        yield return new WaitForSeconds(2.5f);
    }

    public void StartGame ()
    {
        Debug.Log("start");
    }

    public void GoToSettings ()
    {
        UIManager.instance.ShowScreen(ScreenName.Settings);
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
