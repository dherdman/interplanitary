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

    public Vector3 CenterOfMass
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

    public Vector3 FieldStrengthAtPoint(Vector3 point)
    {
        if(point == CenterOfMass)
        {
            Debug.LogError("[GravitationalBody] Attempting to calculate field strength with zero radius (divide by zero)");
            return Vector3.zero; // cannot have same center of mass, use zero
        }

        Vector3 direction = point - CenterOfMass;

        return (G * massiveBody.mass / direction.sqrMagnitude) * direction.normalized;
    }

    public Vector3 GravitationalForce(GravitationalBody other)
    {
        if (other.CenterOfMass == this.CenterOfMass)
        {
            Debug.LogError("[GravitationalBody] Attempting to calculate field strength with zero radius (divide by zero)");
            return Vector3.zero; // cannot have same center of mass, use zero
        }

        return FieldStrengthAtPoint(other.CenterOfMass) * other.massiveBody.mass;
    }
}
