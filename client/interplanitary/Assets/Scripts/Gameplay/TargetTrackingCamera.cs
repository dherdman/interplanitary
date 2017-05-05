using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TargetTrackingCamera : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    Vector3 relativePosition;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void FollowTarget(Transform _target, Vector3 _relativePosition)
    {
        target = _target;
        relativePosition = _relativePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            cam.enabled = true;
            transform.position = target.transform.position - relativePosition;
            transform.LookAt(target);
        }
        else
        {
            cam.enabled = false;
        }
    }
}
