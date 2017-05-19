using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using core;

/// <summary>
/// Handles any ship controls that are independent of the player's current seat/modules
/// </summary>
[RequireComponent(typeof(CameraTarget))]
public class ShipController : MonoBehaviour
{
    CameraTarget _camTarget;
    CameraTarget CamTarget
    {
        get
        {
            if (_camTarget == null)
            {
                _camTarget = GetComponent<CameraTarget>();
            }
            return _camTarget;
        }
    }

    void Awake ()
    {
        // !!! modify to allow spawning in ships
        // !!! will also require changes to PlayerController & probably Ship.Embark
        this.enabled = false;
    }

    void Start()
    {

    }

    void OnEnable()
    {
        CameraManager.instance.AssignPlayerCameraToTarget(CamTarget);
    }

    void Update()
    {

    }
}
