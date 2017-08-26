using UnityEngine;

public abstract class ShipModuleController : MonoBehaviour
{
    void Awake ()
    {
        OnAwake();
    }

    void Start ()
    {
        OnStart();
        this.enabled = false; // by default modules should not be active
    }

    void Update ()
    {
        OnUpdate();
    }

    void FixedUpdate ()
    {
        OnFixedUpdate();
    }

    protected abstract void OnAwake();
    protected abstract void OnStart();
    protected abstract void OnUpdate();
    protected abstract void OnFixedUpdate();

    public abstract void InitializeModule(Ship parentShip);
}
