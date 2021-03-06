﻿
using System;
using UnityEngine;

public class SquareGravitationalBody : GravitationalBody
{
    public override Vector2 FieldStrengthAtPoint(Vector2 point)
    {
        Vector2 direction = CenterOfMass - point;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction.y = 0;
        }
        else
        {
            direction.x = 0;
        }

        return GetFieldStrength(Mass, direction);
    }
}
