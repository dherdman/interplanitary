using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField]
    Camera OverlayCamera;

    [SerializeField]
    TargetTrackingCamera playerCameraPrefab;

    TargetTrackingCamera playerCameraInstance;

    public Camera MainCamera
    {
        get
        {
            return Camera.main;
        }
    }

    Camera _playerCameraComponent;
    public Camera PlayerCamera
    {
        get
        {
            if(_playerCameraComponent == null)
            {
                _playerCameraComponent = playerCameraInstance.GetComponent<Camera>();
            }
            return _playerCameraComponent;
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
        if(playerCameraInstance == null)
        {
            playerCameraInstance = Instantiate(playerCameraPrefab, transform) as TargetTrackingCamera;
        }

        playerCameraInstance.FollowTarget(target, relativePosition);
    }
}
