using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    Camera OverlayCamera;

    [SerializeField]
    TargetTrackingCamera PlayerCamera;

    public Camera MainCamera
    {
        get
        {
            return Camera.main;
        }
    }

    public Camera GetNewNamedOverlayCamera (string name)
    {
        Camera newCam = Instantiate(OverlayCamera, transform);
        newCam.name = string.Format("[Overlay] {0}", name);

        return newCam;
    }

    public void AssignPlayerCameraToTarget(Transform target, Vector3 relativePosition)
    {
        PlayerCamera.FollowTarget(target, relativePosition);
    }
}
