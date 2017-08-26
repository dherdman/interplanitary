using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitModuleController : ShipModuleController
{
    static class ANIM_STATES
    {
        public const string TAKEOFF = "Takeoff";
        public const string LANDING = "Landing";
    }

    Ship PilotedShip;
    Animator ShipAnimator;

    bool launchButtonPressed;

    public override void InitializeModule(Ship parentShip)
    {
        PilotedShip = parentShip;
        ShipAnimator = PilotedShip.GetComponent<Animator>();

        StartLanding();
    }

    protected override void OnAwake() { }
    protected override void OnStart() { }
    protected override void OnUpdate()
    {
        launchButtonPressed = Input.GetButton(InputAxis.ShipControl.TAKEOFF_LANDING);
    }

    protected override void OnFixedUpdate()
    {
        if(launchButtonPressed)
        {
            ShipAnimator.applyRootMotion = true;
            if(PilotedShip.State == ShipState.landed)
            {
                StartTakeoff();
            }
            else
            {
                StartLanding();
            }
        }

        if(PilotedShip.State == ShipState.landing)
        {
            if(PilotedShip.IsLanded)
            {
                ShipAnimator.StopPlayback();
                PilotedShip.OnLandingFinished();
            }
        }
        else if(PilotedShip.State == ShipState.takeoff)
        {
            AnimatorStateInfo stateInfo = ShipAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(ANIM_STATES.TAKEOFF) && stateInfo.normalizedTime > 1)
            {
                PilotedShip.OnTakeoffFinished();
            }
        }
        else if(PilotedShip.IsFlying)
        {
            PilotedShip.transform.position = PilotedShip.transform.position + PilotedShip.transform.up * Input.GetAxis(InputAxis.ShipControl.VERTICAL);
            PilotedShip.transform.position = PilotedShip.transform.position + PilotedShip.transform.right * Input.GetAxis(InputAxis.ShipControl.HORIZONTAL);

            PilotedShip.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, PilotedShip.transform.rotation.eulerAngles.z + Input.GetAxis(InputAxis.ShipControl.ROTATION)));
        }
    }

    void StartTakeoff()
    {
        ShipAnimator.Play(ANIM_STATES.TAKEOFF);
        PilotedShip.OnTakeoffInitiated(); // performs ship state related operations
    }

    void StartLanding()
    {
        ShipAnimator.Play(ANIM_STATES.LANDING);
        PilotedShip.OnLandingInitiated();
    }
}
