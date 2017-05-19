using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitModuleController : ShipModuleController
{
    Ship PilotedShip;

    public override void InitializeModule(Ship parentShip)
    {
        PilotedShip = parentShip;
    }

    protected override void OnAwake() { }
    protected override void OnStart() { }

    protected override void OnUpdate()
    {
        PilotedShip.transform.position = PilotedShip.transform.position + PilotedShip.transform.up * Input.GetAxis(InputAxis.ShipControl.VERTICAL);
        PilotedShip.transform.position = PilotedShip.transform.position + PilotedShip.transform.right * Input.GetAxis(InputAxis.ShipControl.HORIZONTAL);

        //PilotedShip.transform.localPosition = new Vector3(PilotedShip.transform.localPosition.x + Input.GetAxis(InputAxis.ShipControl.HORIZONTAL), PilotedShip.transform.localPosition.y + Input.GetAxis(InputAxis.ShipControl.VERTICAL), 0f);

        PilotedShip.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, PilotedShip.transform.rotation.eulerAngles.z + Input.GetAxis(InputAxis.ShipControl.ROTATION)));
    }
}
