using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{

    public Camera MainCamera
    {
        get
        {
            return Camera.main;
        }
    }

    [SerializeField]
    Camera OverlayCamera;

    public Camera GetNewNamedOverlayCamera (string name)
    {
        Camera newCam = Instantiate(OverlayCamera, transform);
        newCam.name = string.Format("[Overlay] {0}", name);

        return newCam;
    }

}
