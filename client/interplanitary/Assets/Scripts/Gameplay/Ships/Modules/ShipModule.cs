using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShipModule : MonoBehaviour
{
    void Start ()
    {
        OnStart();
        this.enabled = false; // by default modules should not be active
    }

    void Update ()
    {
        OnUpdate();
    }

    protected abstract void OnStart();
    protected abstract void OnUpdate();

    public abstract void InitializeModule(Ship parentShip);
}
