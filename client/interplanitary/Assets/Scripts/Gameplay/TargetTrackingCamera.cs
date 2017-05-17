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
    [SerializeField]
    float transitionTime = 2f;
    [SerializeField]
    AnimationCurve transitionCurve;


    Camera cam;

    float elapsedTransitionTime = 0f;
    Vector3 prevPosition;
    Vector3 prevTargetPosition;
    Vector3 prevTargetUp;

    void Awake()
    {
        cam = GetComponent<Camera>();
        elapsedTransitionTime = transitionTime + 1; // start above transition time (not currently transitioning)
    }

    public void FollowTarget(Transform _target, Vector3 _relativePosition, bool smoothTransition = true)
    {
        if(smoothTransition && target != null)
        {
            prevTargetPosition = target.position;
            prevTargetUp = target.up;
            prevPosition = relativePosition;
            elapsedTransitionTime = 0f;
        }

        target = _target;
        relativePosition = _relativePosition;
    }

    void Update()
    {
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

                relativePos = Vector3.Lerp(prevPosition, relativePosition, t);
                targetPos = Vector3.Lerp(prevTargetPosition, target.position, t);
                targetUp = Vector3.Lerp(prevTargetUp, target.up, t);
            }
            else
            {
                relativePos = relativePosition;
                targetPos = target.position;
                targetUp = target.up;
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
