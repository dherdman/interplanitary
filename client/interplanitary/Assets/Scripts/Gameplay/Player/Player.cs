using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Vector3 cameraPosition;

    void Start()
    {
        CameraManager.instance.AssignPlayerCameraToTarget(transform, cameraPosition);
    }

    void Update()
    {

    }
}
