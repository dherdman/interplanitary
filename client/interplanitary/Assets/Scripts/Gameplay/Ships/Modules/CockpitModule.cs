using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitModule : ShipModule
{
    Ship PilotedShip;

    public override void InitializeModule(Ship parentShip)
    {
        PilotedShip = parentShip;
    }

    protected override void OnStart()
    {

    }

    protected override void OnUpdate()
    {
        PilotedShip.transform.localPosition = new Vector3(PilotedShip.transform.localPosition.x + Input.GetAxis(InputAxis.ShipControl.HORIZONTAL), PilotedShip.transform.localPosition.y + Input.GetAxis(InputAxis.ShipControl.VERTICAL), 0f);
    }
}
