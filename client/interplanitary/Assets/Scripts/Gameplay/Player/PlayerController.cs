using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerController : GravitationalBody
{
    static class ANIM_STATE
    {
        public const string GROUNDED = "Grounded";
        public const string AIR = "Airborne";
        public const string CROUCHED = "Crouched";
    }
    static class ANIM_PARAMS
    {
        // movement
        public const string FORWARD_MOVE = "Forward";
        public const string TURN = "Turn";

        // jumping
        public const string GROUNDED = "OnGround";
        public const string JUMP_LEG = "JumpLeg";
        public const string JUMP = "Jump";
    }

    [Header("Player Movement")]
    [SerializeField]
    float GroundedMoveSpeed;
    [SerializeField]
    float RunCycleLegOffset;
    [SerializeField]
    float AirMoveSpeed;
    [SerializeField]
    float JumpForce;

    const float k_Half = 0.5f;

    Vector3 recentMovementVelocity;

    [Header("Gravity Configuration")]
    [SerializeField]
    float MaxRotationPerSecond;

    float BaseDistToGround;
    float DistToGround;

    Animator animator;

    Plane mouseInteractionPlane;

    protected override void OnAwake()
    {
        DistToGround = GetComponent<Collider>().bounds.extents.y;
        BaseDistToGround = DistToGround;
        animator = GetComponent<Animator>();

        massiveBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        animator.applyRootMotion = false;

        mouseInteractionPlane = new Plane(Vector3.up, Vector3.right, Vector3.zero);
    }

    Vector2? groundedGravity;
    bool IsGrounded
    {
        get
        {
            return groundedGravity.HasValue;
        }
    }

    float MovementSpeed
    {
        get
        {
            return IsGrounded ? GroundedMoveSpeed : AirMoveSpeed;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        GravitationalBody body = col.gameObject.GetComponent<GravitationalBody>();
        if (body != null && col.contacts.Length > 0)
        {
            animator.applyRootMotion = true;

            ContactPoint contact = col.contacts[0];
            Vector2 normal2d = contact.normal;
            groundedGravity = Vector2.Dot(body.GravitationalPull(this), normal2d) * normal2d;

            // !!! TODO queue landing animations

            animator.SetFloat(ANIM_PARAMS.JUMP, 0); // kill jump motion on snap to ground
            SmoothRotateParallel(groundedGravity.Value, true);
            //transform.position = contact.point + contact.normal * DistToGround; // !!! not needed? 
        }
    }

    void OnCollisionExit(Collision col)
    {
        GravitationalBody body = col.gameObject.GetComponent<GravitationalBody>();
        if (body != null)
        {
            groundedGravity = null;
            animator.applyRootMotion = false;
        }
    }

    void FixedUpdate()
    {
        MoveCharacter();

        LookAtMouse();
    }

    void MoveCharacter()
    {
        animator.SetBool(ANIM_PARAMS.GROUNDED, IsGrounded);

        float moveAmount = transform.forward.x * Input.GetAxis(InputAxis.PlayerControl.HORIZONTAL);

        if (IsAnimatorState(ANIM_STATE.GROUNDED))
        {
            animator.SetFloat(ANIM_PARAMS.FORWARD_MOVE, moveAmount);
        }

        if (!IsGrounded)
        {
            HandleAirborneMovement();
        }
        else
        {
            HandleGroundedMovement(moveAmount, Input.GetButton(InputAxis.PlayerControl.JUMP));
        }
    }

    //void CheckGrounded()
    //{
    //    RaycastHit hitInfo;

    //    if(Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out hitInfo, DistToGround))
    //    {
    //        if(hitInfo.collider.gameObject.layer == Layers.ID.Worlds)
    //        {
    //            GravitationalBody body = hitInfo.collider.gameObject.GetComponent<GravitationalBody>();
    //            if (body != null)
    //            {
    //                Debug.Log("Grounded");
    //                IsGrounded = true;
    //                animator.applyRootMotion = true;

    //                Vector2 normal2d = hitInfo.normal;
    //                groundedGravity = Vector2.Dot(body.GravitationalPull(this), normal2d) * normal2d;

    //                transform.position = hitInfo.point;

    //                SmoothRotateParallel(groundedGravity.Value, true);
    //            };
    //        }
    //    }
    //    else
    //    {
    //        IsGrounded = false;
    //        animator.applyRootMotion = false;
    //    }

    //}

    void HandleGroundedMovement(float moveAmount, bool jump)
    {
        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime + RunCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * moveAmount;

        animator.SetFloat(ANIM_PARAMS.JUMP_LEG, jumpLeg);

        if (jump && IsGrounded && IsAnimatorState(ANIM_STATE.GROUNDED)) // ensure a jump is valid
        {
            massiveBody.AddForce(transform.up * JumpForce);
        }
    }

    void HandleAirborneMovement()
    {
        ApplyGravity();
        animator.SetFloat(ANIM_PARAMS.JUMP, massiveBody.velocity.y);

        DistToGround = massiveBody.velocity.y < 0 ? BaseDistToGround : 0.01f;
    }

    void ApplyGravity()
    {
        GravitationalBody[] bodies = FindObjectsOfType<GravitationalBody>();

        Vector2 netForce = new Vector3();
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != this && bodies[i].isActiveAndEnabled)
            {
                Vector2 force = bodies[i].GravitationalPull(this);
                netForce += force;
            }
        }

        SmoothRotateParallel(netForce, true); // use net force to get "up" direction

        massiveBody.AddForce(Vector2.Dot(-transform.up, netForce) * -transform.up); // applied force is only ever "down"
    }

    bool IsAnimatorState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    void LookAtMouse()
    {
        //Ray ray = CameraManager.instance.PlayerCamera.ScreenPointToRay(Input.mousePosition);

        //float distance;
        //if (mouseInteractionPlane.Raycast(ray, out distance))
        //{
        //    Vector3 hitPoint = ray.GetPoint(distance); // !!! store whole vector b/c y will be used for aiming later

        //    float turn;
        //    if (transform.forward.x == Mathf.Sign(hitPoint.x - transform.position.x)) // if already facing correct direction, no need to turn
        //    {
        //        turn = 0;
        //    }
        //    else
        //    {
        //        turn = -transform.forward.x;
        //    }

        //    animator.SetFloat(ANIM_PARAMS.TURN, turn);
        //}

        if (transform.forward.x * (Input.mousePosition.x / Screen.width < 0.5f ? -1 : 1) < 0) // if transform.forward is not the same direction as the mouse, rotate 180
        {
            Vector3 newRot = transform.rotation.eulerAngles;
            newRot.y = newRot.y + 180 % 360;
            transform.rotation = Quaternion.Euler(newRot);
        }
    }

    void SmoothRotateParallel(Vector2 gravity, bool snap)
    {
        if (gravity.magnitude > 0)
        {
            float delta = Vector2.Angle(transform.up, -gravity); // absolute degree change

            if (!snap)
            {
                delta *= Time.fixedDeltaTime; // change in "up" in degrees/second

                if (MaxRotationPerSecond > 0 && Mathf.Abs(delta) > MaxRotationPerSecond)
                {
                    delta = delta < 0 ? -MaxRotationPerSecond : MaxRotationPerSecond;
                }
            }

            Vector3 rotation = transform.rotation.eulerAngles + new Vector3(0, 0, delta);

            transform.rotation = Quaternion.Euler(rotation);
        }
    }

}
