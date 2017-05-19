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

    protected abstract void OnAwake();
    protected abstract void OnStart();
    protected abstract void OnUpdate();

    public abstract void InitializeModule(Ship parentShip);
}
