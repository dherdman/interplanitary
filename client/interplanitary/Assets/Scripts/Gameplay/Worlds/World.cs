using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : SquareGravitationalBody
{
    protected override void OnAwake()
    {

    }

    void Update()
    {
        Debug.DrawLine(transform.position - new Vector3(-1, -1, 0) * 100, transform.position - new Vector3(1, 1, 0) * 100);
        Debug.DrawLine(transform.position - new Vector3(-1, 1, 0) * 100, transform.position - new Vector3(1, -1, 0) * 100);
    }
}
