using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerController : GravitationalBody
{
    [Header("Player Movement")]
    [SerializeField]
    float GroundedMoveSpeed;
    [SerializeField]
    float AirMoveSpeed;

    Vector3 recentMovementVelocity;

    [Header("Gravity Configuration")]
    [SerializeField]
    float MaxRotationPerSecond;

    float DistToGround;

    protected override void OnAwake()
    {
        DistToGround = GetComponent<Collider>().bounds.extents.y;
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
            ContactPoint contact = col.contacts[0];
            Vector2 normal2d = contact.normal;
            groundedGravity = Vector2.Dot(body.GravitationalPull(this), normal2d) * normal2d;

            transform.position = contact.point + contact.normal * DistToGround;

            // !!! TODO snap to ground on hit
            SmoothRotateParallel(groundedGravity.Value, true);
        }
    }

    void OnCollisionExit(Collision col)
    {
        GravitationalBody body = col.gameObject.GetComponent<GravitationalBody>();
        if (body != null)
        {
            groundedGravity = null;
        }
    }

    void FixedUpdate()
    {
        if (!IsGrounded)
        {
            ApplyGravity();
        }

        MoveCharacter(); // !!! TODO move to animation system
    }

    void MoveCharacter()
    {
        massiveBody.AddForce(-recentMovementVelocity, ForceMode.VelocityChange);
        recentMovementVelocity = MovementSpeed * Input.GetAxis(InputAxis.PlayerControl.HORIZONTAL) * transform.right;
        massiveBody.AddForce(recentMovementVelocity, ForceMode.VelocityChange);
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

        SmoothRotateParallel(netForce, false);

        massiveBody.AddForce(netForce);
    }

    void SmoothRotateParallel(Vector2 gravity, bool snap)
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

        massiveBody.MoveRotation(Quaternion.Euler(rotation));
    }

}
