using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GenericCharacterController
{
    // !!! TODO temp until animated?
    float nextWeaponSwapTime = 0f;
    float weaponSwapCooldown = 0.1f;

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

    bool jumpButtonPressed;
    float inputMoveAmount;

    Animator animator;

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

    Plane mouseInteractionPlane;

    List<IInteractable> currentlyInteractableObjects;

    protected override void OnAwake()
    {
        base.OnAwake();

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        mouseInteractionPlane = new Plane(Vector3.up, Vector3.right, Vector3.zero);

        currentlyInteractableObjects = new List<IInteractable>();
    }

    protected override void OnEnabledCallback()
    {
        CameraManager.instance.AssignPlayerCameraToTarget(CamTarget);
    }
    protected override void OnDisabledCallback() { }

    void OnTriggerEnter(Collider col)
    {
        IInteractable interactableObj = col.gameObject.GetComponent<IInteractable>();
        if (interactableObj != null)
        {
            currentlyInteractableObjects.Add(interactableObj); // !!! TODO add system for prioritizing or rotating interactions
        }
    }

    void OnTriggerExit(Collider col)
    {
        IInteractable interactableObj = col.gameObject.GetComponent<IInteractable>();

        if (interactableObj != null)
        {
            currentlyInteractableObjects.Remove(interactableObj);
        }
    }

    void Update()
    {
        if(Input.GetButton(InputAxis.PlayerControl.INVENTORY))
        {
            //UIManager.instance.ToggleOverlay(UIManager.Overlays.Inventory);
        }
        else
        {
            if (currentlyInteractableObjects.Count > 0 && Input.GetButton(InputAxis.PlayerControl.INTERACT))
            {
                currentlyInteractableObjects[0].Interact(CharacterInstance);
                currentlyInteractableObjects.RemoveAt(0);
            }

            inputMoveAmount = Input.GetAxis(InputAxis.PlayerControl.HORIZONTAL);
            jumpButtonPressed = Input.GetButtonDown(InputAxis.PlayerControl.JUMP);

            float equippedSwap = Input.GetAxis(InputAxis.PlayerControl.EQUIPPED_SWAP);

            if (equippedSwap != 0 && Time.time > nextWeaponSwapTime)
            {
                nextWeaponSwapTime = Time.time + weaponSwapCooldown;
                // !!! TODO swap weapon
                CharacterInstance.CycleWeapon(equippedSwap < 0);
            }
            // else if so that firing cannot occur on the same frame as swapping weapons
            else if(Input.GetButton(InputAxis.PlayerControl.FIRE))
            {
                CharacterInstance.UsePrimary();
            }
        }

    }

    bool IsFacingRight
    {
        get
        {
            Vector3 t = transform.forward;
            Vector3 cam = ScreenRight;

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

    void ProcessGrounded()
    {
        if(IsGrounded)
        {
            animator.applyRootMotion = true;
            animator.SetFloat(ANIM_PARAMS.JUMP, 0); // kill jump on landing
            // TODO play some landing animations
        }
        else
        {
            animator.applyRootMotion = false;
        }
    }

    protected override void OnFixedUpdate()
    {
        ProcessGrounded();
        LookAtMouse();
    }

    protected override void MoveCharacter()
    {
        animator.SetBool(ANIM_PARAMS.GROUNDED, IsGrounded);

        transform.position = new Vector3(transform.position.x, transform.position.y, 0); // !!! hack b/c current animation does not stay in z plane

        if (!IsGrounded)
        {
            HandleAirborneMovement(inputMoveAmount);
        }
        else
        {
            HandleGroundedMovement(inputMoveAmount, jumpButtonPressed);
        }
        ApplyGravity();
    }

    void HandleGroundedMovement(float moveAmount, bool jump)
    {
        moveAmount = (IsFacingRight ? 1 : -1) * moveAmount;

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
            ownRigidBody.AddForce(transform.up * jumpForce);
        }
    }

    void HandleAirborneMovement(float moveAmount)
    {
        // move only perpendicular to gravity 
        float perpVelocity = Vector3.Dot(ownRigidBody.velocity, NetGravityPerpendicular);
        float perpMove = moveAmount * airMoveSpeed;

        if (perpMove != 0 && (perpMove < 0 ? perpMove < perpVelocity : perpMove > perpVelocity))
        {
            // if move input is greater than current velocity, remove current velocity and add move velocity (add the difference)
            ownRigidBody.velocity = ownRigidBody.velocity + (Vector3)NetGravityPerpendicular * (perpMove - perpVelocity);
        }

        animator.SetFloat(ANIM_PARAMS.JUMP, Vector3.Dot(ownRigidBody.velocity, NetGravity)); // get speed in the "down" direction
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

            // !!!!! TODO(also in player) should be animation based, this is temp
            CharacterInstance.AimAt(hitPoint);

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
    }

    /// <summary>
    /// Performs any necessary cleanup when disabling player control upon entering a vehicle
    /// </summary>
    public override void DisableSelf()
    {
        // reset animator state
        animator.SetFloat(ANIM_PARAMS.FORWARD_MOVE, 0);
        animator.SetFloat(ANIM_PARAMS.JUMP, 0f);
        animator.SetFloat(ANIM_PARAMS.JUMP_LEG, 0f);
        animator.SetFloat(ANIM_PARAMS.TURN, 0f);

        animator.SetBool(ANIM_PARAMS.GROUNDED, true);

        base.DisableSelf();
    }
}
