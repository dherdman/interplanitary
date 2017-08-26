using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : Singleton<GravityManager>
{
    List<GravitationalBody> activeBodies;

    void Start()
    {
        activeBodies = new List<GravitationalBody>();
    }

    public Vector2 NetGravityAtPoint(Vector2 point, float mass, List<int> validLayers = null)
    {
        return mass * NetFieldStrengthAtPoint(point, validLayers);
    }

    public Vector2 NetFieldStrengthAtPoint(Vector2 point, List<int> validLayers = null)
    {
        Vector2 fieldStrength = Vector2.zero;
        for (int i = 0; i < activeBodies.Count; i++)
        {
            if ((validLayers == null || validLayers.Contains(activeBodies[i].gameObject.layer)) && activeBodies[i].isActiveAndEnabled)
            {
                Vector2 strength = activeBodies[i].FieldStrengthAtPoint(point);
                fieldStrength += strength;
            }
        }
        return fieldStrength;
    }

    public void RegisterBody(GravitationalBody body)
    {
        activeBodies.Add(body);
    }

    public void DeregisterBody(GravitationalBody body)
    {
        activeBodies.Remove(body);
    }
}
