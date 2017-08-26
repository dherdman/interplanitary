using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalBody : MonoBehaviour
{
    const float G = 1; // gravitational constant in standard form

    [SerializeField]
    protected Vector2 localCenterOfMass;
    public Vector2 CenterOfMass
    {
        get
        {
            return (Vector2)transform.position + localCenterOfMass;
        }
    }

    [SerializeField]
    protected float mass;

    public float Mass
    {
        get
        {
            return mass;
        }
    }

    void Awake()
    {
        OnAwake();
    }

    void OnEnable()
    {
        if (GravityManager.instance != null)
        {
            GravityManager.instance.RegisterBody(this);
        }

        OnEnabledCallback();
    }

    void OnDisable()
    {
        if(GravityManager.instance != null)
        {
            GravityManager.instance.DeregisterBody(this);
        }

        OnDisabledCallback();
    }

    protected virtual void OnAwake() { }
    protected virtual void OnEnabledCallback() { }
    protected virtual void OnDisabledCallback() { }

    public virtual Vector2 FieldStrengthAtPoint(Vector2 point)
    {
        if (point == CenterOfMass)
        {
            Debug.LogError("[GravitationalBody] Attempting to calculate field strength with zero radius (divide by zero)");
            return Vector2.zero; // cannot have same center of mass, return zero force instead of infinite
        }

        Vector2 direction = CenterOfMass - point;

        return GetFieldStrength(Mass, direction);
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

        return FieldStrengthAtPoint(other.CenterOfMass) * other.Mass;
    }

}
