using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerController : GravitationalBody
{
    [SerializeField]
    float MaxRotationPerFrame;

    float DistToGround;

    Vector3 GravitationalDown;
    Collider PlayerCollider;

    protected override void OnAwake()
    {
        DistToGround = GetComponent<Collider>().bounds.extents.y;
    }

    bool _grounded = false;
    bool IsGrounded // !!! TODO fix spazzy rotation on hit
    {
        get
        {
            return _grounded;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer(Layers.Worlds))
        {
            _grounded = true;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(Layers.Worlds))
        {
            _grounded = false;
        }
    }

    void FixedUpdate()
    {
        GravitationalBody[] bodies = FindObjectsOfType<GravitationalBody>();

        Vector3 netForce = new Vector3();
        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i] != this)
            {
                Vector3 force = this.GravitationalForce(bodies[i]);
                netForce += force;
            }
        }

        SmoothRotateParallel(netForce);

        massiveBody.AddForce(netForce);
    }

    void SmoothRotateParallel(Vector3 parallel)
    {
        Vector3 down = transform.rotation.eulerAngles - parallel;

        if (down.magnitude > MaxRotationPerFrame)
        {
            down = down.normalized * MaxRotationPerFrame;
        }

        transform.rotation = Quaternion.FromToRotation(Vector3.right, down);
    }

}
