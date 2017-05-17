using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
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
    float runCycleLegOffset;
    [SerializeField]
    float airMoveSpeed;
    [SerializeField]
    float jumpForce;

    const float k_Half = 0.5f;

    [Header("Gravity Configuration")]
    [SerializeField]
    float maxDegreeRotationPerFixedUpdate;

    float distToGround;
    float colliderWidth;

    bool IsGrounded;
    Vector2 NetGravity;
    Vector2 NetGravityPerpendicular
    {
        get
        {
            return new Vector2(-NetGravity.y, NetGravity.x).normalized;
        }
    }

    Animator animator;
    Rigidbody playerRigidBody;

    Plane mouseInteractionPlane;

    protected override void OnAwake()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y / 4;
        colliderWidth = GetComponent<Collider>().bounds.extents.x;

        playerRigidBody = GetComponent<Rigidbody>();
        playerRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        mouseInteractionPlane = new Plane(Vector3.up, Vector3.right, Vector3.zero);
    }

    bool IsFacingRight
    {
        get
        {
            Vector3 t = transform.forward;
            Vector3 cam = ScreenRight;

            //Debug.Log(t + " " + cam);
            //Debug.Log((Sign(t.x) == Sign(cam.x)) + " " + (Sign(t.y) == Sign(cam.y)));

            return core.Math.Sign(t.x) == core.Math.Sign(cam.x) && core.Math.Sign(t.y) == core.Math.Sign(cam.y);
        }
    }
    Vector2 ScreenRight
    {
        get
        {
            return CameraManager.instance.PlayerCamera.transform.right;
        }
    }
    
    void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * distToGround + transform.forward * colliderWidth, -transform.up, out hit, distToGround * 2) ||
           Physics.Raycast(transform.position + transform.up * distToGround - transform.forward * colliderWidth, -transform.up, out hit, distToGround * 2))
        {
            IsGrounded = true;
            animator.applyRootMotion = true;
            animator.SetFloat(ANIM_PARAMS.JUMP, 0); // kill jump on landing
            // TODO play some landing animations
        }
        else
        {
            IsGrounded = false;
            animator.applyRootMotion = false;
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        LookAtMouse();
        MoveCharacter();
    }

    void MoveCharacter()
    {
        animator.SetBool(ANIM_PARAMS.GROUNDED, IsGrounded);

        float moveAmount = (IsFacingRight ? 1 : -1) * Input.GetAxis(InputAxis.PlayerControl.HORIZONTAL);

        transform.position = new Vector3(transform.position.x, transform.position.y, 0); // !!! hack b/c current animation does not stay in z plane

        if (!IsGrounded)
        {
            HandleAirborneMovement(moveAmount);
        }
        else
        {
            HandleGroundedMovement(moveAmount, Input.GetButton(InputAxis.PlayerControl.JUMP));
        }
        ApplyGravity();
    }

    void HandleGroundedMovement(float moveAmount, bool jump)
    {
        animator.SetFloat(ANIM_PARAMS.FORWARD_MOVE, moveAmount);

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime + runCycleLegOffset, 1);
        float jumpLeg = (runCycle < k_Half ? 1 : -1) * moveAmount;

        animator.SetFloat(ANIM_PARAMS.JUMP_LEG, jumpLeg);

        if (jump && IsGrounded && IsAnimatorState(ANIM_STATE.GROUNDED)) // ensure a jump is valid
        {
            playerRigidBody.AddForce(transform.up * jumpForce);
        }
    }

    void HandleAirborneMovement(float moveAmount)
    {
        // move only perpendicular to gravity 

        float perpVelocity = Vector3.Dot(playerRigidBody.velocity, NetGravityPerpendicular);
        float perpMove = moveAmount * airMoveSpeed;

        Debug.Log(perpVelocity + " " + perpMove);

        if(moveAmount != 0 && (moveAmount < 0 ? moveAmount < perpVelocity : moveAmount > perpVelocity))
        {
            // if move input is greater than current velocity, remove current velocity and add move velocity (add the difference)
            playerRigidBody.velocity = playerRigidBody.velocity + (Vector3)NetGravityPerpendicular * (perpMove - perpVelocity);
        }

        animator.SetFloat(ANIM_PARAMS.JUMP, Vector3.Dot(playerRigidBody.velocity, NetGravity)); // get speed in the "down" direction
    }

    void ApplyGravity()
    {
        GravitationalBody[] bodies = FindObjectsOfType<GravitationalBody>();

        NetGravity = new Vector3();
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != this && bodies[i].isActiveAndEnabled)
            {
                Vector2 force = bodies[i].GravitationalPull(this);
                NetGravity += force;
            }
        }

        SmoothRotateParallel(NetGravity, false); // use net force to get "up" direction

        playerRigidBody.AddForce(NetGravity); 
    }

    bool IsAnimatorState(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    void LookAtMouse()
    {
        Ray ray = CameraManager.instance.PlayerCamera.ScreenPointToRay(Input.mousePosition);

        float distance;
        if (mouseInteractionPlane.Raycast(ray, out distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance); // !!! store whole vector b/c y will be used for aiming later

            // !!! TODO animate turn ?
            //float turn;
            if (Mathf.Abs(Vector3.Angle(transform.forward, (hitPoint - transform.position))) > 90) // if already facing correct direction, no need to turn
            {
                transform.RotateAround(transform.position, transform.up, 180);
                //turn = -transform.forward.x;
            }
            else
            {
                //turn = 0;
            }

        }

        // == old method, has issues when transform.forward is down/up (x component ~= 0)
        //if (transform.forward.x * (Input.mousePosition.x / Screen.width < 0.5f ? -1 : 1) < 0) // if transform.forward is not the same direction as the mouse, rotate 180
        //{
        //    transform.RotateAround(transform.position, transform.up, 180);
        //}
    }

    void SmoothRotateParallel(Vector2 gravity, bool snap)
    {
        if (gravity.magnitude > 0)
        {
            // angle between transform up and the direction of gravity
            float delta = Vector2.Angle(-transform.up, gravity); // absolute degree change

            delta *= core.Math.Sign(Vector3.Cross(-transform.up, gravity).z, false); // find direction using cross product of Vector3s in the X-Y Plane

            if (core.Math.Sign(transform.right.z) == -1 ^ core.Math.Sign(transform.up.y) == -1)
            {
                delta *= -1;
            }

            if (!snap)
            {
                if (maxDegreeRotationPerFixedUpdate > 0 && Mathf.Abs(delta) > maxDegreeRotationPerFixedUpdate)
                {
                    delta = Mathf.Clamp(delta, -maxDegreeRotationPerFixedUpdate, maxDegreeRotationPerFixedUpdate);
                }
            }

            Vector3 rotation = transform.rotation.eulerAngles + new Vector3(delta, 0, 0); // !!! x and z will swap once a custom model is in place

            transform.rotation = Quaternion.Euler(rotation);
        }
    }

}
