using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ship hull should cover the entire collidable area of the ship
/// !!! may refactor such that hull, engine, guns, etc are separate and call this something else like container
/// </summary>
[RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class ShipHull : MonoBehaviour
{
    public delegate void CollideWithWorldCallback();
    public CollideWithWorldCallback OnWorldCollisionEntered;
    public CollideWithWorldCallback OnWorldCollisionExited;

    public delegate void AtmosphereChangeCallback();
    public AtmosphereChangeCallback OnAtmosphereEntered;
    public AtmosphereChangeCallback OnAtmosphereExited;

    Rigidbody shipRigidBody;

    void Awake()
    {
        shipRigidBody = GetComponent<Rigidbody>();
        shipRigidBody.constraints = RigidbodyConstraints.FreezeAll;

        // !!! these may change w/ addition of in atmosphere gravity
        shipRigidBody.useGravity = false;
        shipRigidBody.isKinematic = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.layer == Layers.ID.Worlds)
        {
            if (OnWorldCollisionEntered != null)
            {
                OnWorldCollisionEntered();
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.collider.gameObject.layer == Layers.ID.Worlds)
        {
            if (OnWorldCollisionExited != null)
            {
                OnWorldCollisionExited();
            }
        }
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.layer == Layers.ID.Atmosphere)
        {
            if (OnAtmosphereEntered != null)
            {
                OnAtmosphereEntered();
            }
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.layer == Layers.ID.Atmosphere)
        {
            if (OnAtmosphereExited != null)
            {
                OnAtmosphereExited();
            }
        }
    }
}
