using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TargetTrackingCamera : MonoBehaviour
{
    [SerializeField]
    CameraTarget target;
    [SerializeField]
    Vector3 closeRelativePosition;
    [SerializeField]
    Vector3 farRelativePosition;

    float cameraZoomPosition;

    Vector3 RelativePosition
    {
        get
        {
            return Vector3.Lerp(farRelativePosition, closeRelativePosition, cameraZoomPosition);
        }
    }

    Camera cam;

    [SerializeField]
    float transitionTime = 2f;
    [SerializeField]
    AnimationCurve transitionCurve;

    float elapsedTransitionTime = 0f;
    Vector3 prevPosition;
    Vector3 prevTargetPosition;
    Vector3 prevTargetUp;

    void Awake()
    {
        cameraZoomPosition = 1f; // start at closeRelativePosition !!! -> player preference?

        cam = GetComponent<Camera>();
        elapsedTransitionTime = transitionTime + 1; // start above transition time (not currently transitioning)
    }

    public void FollowTarget(CameraTarget _target, bool smoothTransition)
    {
        if(smoothTransition && target != null)
        {
            prevTargetPosition = target.transform.position;
            prevTargetUp = target.transform.up;
            prevPosition = RelativePosition;
            elapsedTransitionTime = 0f;
        }

        target = _target;
        closeRelativePosition = _target.closeRelativePosition;
        farRelativePosition = _target.farRelativePosition;
    }

    void Update()
    {
        //cameraZoomPosition = Mathf.Clamp(cameraZoomPosition + Input.GetAxisRaw(InputAxis.General.CAMERA_ZOOM), 0f, 1f); // !!! TODO rework to not use mouse wheel, used for items instead

        if (target != null)
        {
            cam.enabled = true;
            Vector3 relativePos;
            Vector3 targetPos;
            Vector3 targetUp;

            if(elapsedTransitionTime < transitionTime)
            {
                elapsedTransitionTime += Time.deltaTime;
                float t = transitionCurve.Evaluate(elapsedTransitionTime / transitionTime);

                relativePos = Vector3.Lerp(prevPosition, RelativePosition, t);
                targetPos = Vector3.Lerp(prevTargetPosition, target.transform.position, t);
                targetUp = Vector3.Lerp(prevTargetUp, target.transform.up, t);
            }
            else
            {
                relativePos = RelativePosition;
                targetPos = target.transform.position;
                targetUp = target.transform.up;
            }

            transform.position = targetPos - relativePos;
            transform.LookAt(targetPos, targetUp);
        }
        else
        {
            cam.enabled = false;
        }
    }
}
