using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CameraTarget))]
public class GenericCharacterController : GravitationalBody
{
    [Header("Gravity Configuration")]
    [SerializeField]
    float maxDegreeRotationPerFixedUpdate;

    float distToGround;
    float colliderWidth;

    protected bool IsGrounded;

    protected Vector2 NetGravity;
    protected Vector2 NetGravityPerpendicular
    {
        get
        {
            return new Vector2(-NetGravity.y, NetGravity.x).normalized;
        }
    }

    protected Rigidbody ownRigidBody;

    protected Character CharacterInstance;

    protected override void OnAwake()
    {
        Collider c = GetComponent<Collider>();
        distToGround = c.bounds.extents.y / 4;
        colliderWidth = c.bounds.extents.x;

        ownRigidBody = GetComponent<Rigidbody>();
        ownRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        CharacterInstance = GetComponent<Character>();
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up * distToGround + transform.forward * colliderWidth, -transform.up, out hit, distToGround * 2, LayerMask.GetMask(Layers.Worlds)) ||
           Physics.Raycast(transform.position + transform.up * distToGround - transform.forward * colliderWidth, -transform.up, out hit, distToGround * 2, LayerMask.GetMask(Layers.Worlds)))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    protected virtual void MoveCharacter()
    {
        // Should be overwritten by the non-generic controller
    }

    void FixedUpdate()
    {
        CheckGrounded();
        MoveCharacter();

        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {

    }

    protected void ApplyGravity()
    {
        NetGravity = GravityManager.instance.NetGravityAtPoint(CenterOfMass, Mass, new List<int> { Layers.ID.Worlds });

        SmoothRotateParallel(NetGravity, false); // use net force to get "up" direction

        ownRigidBody.AddForce(NetGravity);
    }

    void SmoothRotateParallel(Vector2 gravity, bool snap)
    {
        if (gravity.magnitude > 0)
        {
            // angle between transform up and the direction of gravity
            float delta = core.Math.SignedAngle(-transform.up, gravity);

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

    public virtual void DisableSelf()
    {
        // stop all rigid body motion
        ownRigidBody.velocity = Vector3.zero;

        // disable script
        this.enabled = false;
    }
}
