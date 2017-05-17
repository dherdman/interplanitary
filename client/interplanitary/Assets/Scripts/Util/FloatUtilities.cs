using UnityEngine;

public static class FloatUtilities
{
    public static bool RoughlyZero(float num)
    {
        return Mathf.Abs(num) < 0.001;
    }
}
