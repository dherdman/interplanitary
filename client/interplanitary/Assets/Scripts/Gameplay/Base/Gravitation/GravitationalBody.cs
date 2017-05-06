using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class GravitationalBody : MonoBehaviour
{
    const float G = 1; // gravitational constant in standard form

    public Rigidbody massiveBody // don't call this rigidBody b/c too similar to [deprecated] field "rigidbody" - 20170504
    {
        get; private set;
    }

    public Vector2 CenterOfMass
    {
        get
        {
            return transform.position + massiveBody.centerOfMass;
        }
    }

    void Awake()
    {
        massiveBody = GetComponent<Rigidbody>();

        OnAwake();
    }

    protected abstract void OnAwake();

    public virtual Vector2 FieldStrengthAtPoint(Vector2 point)
    {
        if (point == CenterOfMass)
        {
            Debug.LogError("[GravitationalBody] Attempting to calculate field strength with zero radius (divide by zero)");
            return Vector2.zero; // cannot have same center of mass, return zero force instead of infinite
        }

        Vector2 direction = CenterOfMass - point;

        return GetFieldStrength(massiveBody.mass, direction);
    }

    protected Vector2 GetFieldStrength(float mass, Vector2 direction)
    {
        return (G * mass / direction.sqrMagnitude) * direction.normalized;
    }

    public virtual Vector2 GravitationalPull(GravitationalBody other)
    {
        if (other.CenterOfMass == this.CenterOfMass)
        {
            Debug.LogError("[GravitationalBody] Attempting to calculate field strength with zero radius (divide by zero)");
            return Vector2.zero; // cannot have same center of mass, return zero force instead of infinite
        }

        return FieldStrengthAtPoint(other.CenterOfMass) * other.massiveBody.mass;
    }

}
