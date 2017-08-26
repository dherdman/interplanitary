using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipSeat
{
    public Player occupant;
    // !!! perhaps this should be an array of roles
    public ShipSeatRole role;
    public List<ShipModuleController> controlledModules;
}

/// <summary>
/// Lower indexes are higher priority seats. New seat types must be placed in their proper hierarchical position
/// </summary>
public enum ShipSeatRole : int
{
    pilot,
    gunner,
    passenger
}

public enum ShipState
{
    landed,
    landing,
    takeoff,
    planitary_flight,
    space_flight
}

public enum ShipWeaponsState
{
    hot,
    cold
}

[RequireComponent(typeof(ShipController))]
public class Ship : MonoBehaviour, IInteractable
{
    [Header("Seating Configuration")]
    [SerializeField]
    bool boardable = true;

    [SerializeField]
    List<ShipSeat> Seats;
    ShipSeat HighestPriorityAvailableSeat
    {
        get
        {
            ShipSeat seat = null;
            for (int i = 0; i < Seats.Count; i++)
            {
                if (seat == null || (seat.occupant == null && seat.role - Seats[i].role > 0))
                {
                    seat = Seats[i];
                }
            }
            return seat;
        }
    }

    [SerializeField]
    public ShipStats Stats;

    [SerializeField]
    ShipHull hull;

    public ShipState State { get; private set; }

    public bool IsFlying
    {
        get
        {
            return !IsLanded;
        }
    }
    public bool IsLanded
    {
        get; private set;
    }

    ShipController shipController;

    int availableSeats;

    int _numEnteredAtmospheres = 0;
    bool IsInAtmosphere
    {
        get
        {
            return _numEnteredAtmospheres > 0;
        }
    }

    public bool CanInteract(Player playerInstance)
    {
        // !!! could add in some level caps or something to this for more variety & player progression
        return boardable && availableSeats > 0;
    }

    public void Interact(Player playerInstance)
    {
        if (CanInteract(playerInstance))
        {
            Embark(playerInstance);
        }
    }

    // Because the ship uses an interaction trigger, it cannot properly detect collisions with triggers, so let the hull manage that and subscribe to events
    void OnAtmosphereEntered()
    {
        _numEnteredAtmospheres++;

        if (State == ShipState.space_flight)
        {
            State = ShipState.planitary_flight;
        }
    }

    // Because the ship uses an interaction trigger, it cannot properly detect collisions with triggers, so let the hull manage that and subscribe to events
    void OnAtmosphereExited()
    {
        _numEnteredAtmospheres--;

        if (State == ShipState.planitary_flight)
        {
            State = ShipState.space_flight;
        }
    }

    void OnWorldHit()
    {
        IsLanded = true;
    }

    void OnWorldExit()
    {
        IsLanded = false;
    }

    void Awake()
    {
        if (hull == null)
        {
            Debug.LogWarning("[Ship] Ship's hull was not assigned to serialized field, using Object.Find to get it");

            hull = gameObject.GetComponentInChildren<ShipHull>();

            if (hull == null)
            {
                Debug.LogError("[Ship] Ship hull not found after searching!");
            }
        }

        hull.OnAtmosphereEntered += OnAtmosphereEntered;
        hull.OnAtmosphereExited += OnAtmosphereExited;
    }

    void Start()
    {
        shipController = GetComponent<ShipController>();

        if (Seats.Count < 1)
        {
            GameObject moduleHolder = new GameObject();
            moduleHolder.transform.parent = transform;
            moduleHolder.transform.position = Vector3.zero;
            CockpitModuleController module = moduleHolder.AddComponent<CockpitModuleController>();
            module.InitializeModule(this);

            Seats = new List<ShipSeat> {
                new ShipSeat
                {
                    occupant = null,
                    role = ShipSeatRole.pilot,
                    controlledModules = new List<ShipModuleController> { module },
                }
            };
        }
        else
        {
            Seats.Sort((a, b) =>
            {
                return a.role - b.role;
            });

            InitializeSeatModules(Seats[0]);
            for (int i = 1; i < Seats.Count; i++)
            {
                if (Seats[i].role == ShipSeatRole.pilot)
                {
                    Debug.LogWarning("[Ship] Ship configured to have multiple pilots. Changing pilot at seat " + i + " to a passenger");
                    Seats[i].role = ShipSeatRole.passenger;

                    // more specific version of InitializeSeatModules that will remove CockpitModules as well as nulls
                    for (int j = Seats[i].controlledModules.Count - 1; j >= 0; j--)
                    {
                        if (Seats[i].controlledModules[j] == null || Seats[i].controlledModules[j].GetType() == typeof(CockpitModuleController))
                        {
                            Seats[i].controlledModules.RemoveAt(j);
                        }
                        else
                        {
                            Seats[i].controlledModules[j].InitializeModule(this);
                        }
                    }
                }
                else
                {
                    InitializeSeatModules(Seats[i]);
                }
            }
        }

        availableSeats = Seats.Count;
    }

    void Update()
    {

    }

    void InitializeSeatModules(ShipSeat seat)
    {
        for (int i = seat.controlledModules.Count - 1; i >= 0; i--)
        {
            if (seat.controlledModules[i] == null)
            {
                seat.controlledModules.RemoveAt(i);
            }
            seat.controlledModules[i].InitializeModule(this);
        }
    }

    // !!! TODO different handling for controlled players vs uncontrolled
    void Embark(Player playerInstance)
    {
        ShipSeat playerSeat = HighestPriorityAvailableSeat;

        playerSeat.occupant = playerInstance;

        shipController.enabled = true;
        EnableModulesForSeat(playerSeat);
        playerInstance.ToggleControl();

        availableSeats--;
    }

    void Disembark(Player playerInstance)
    {
        shipController.enabled = false;

        for (int i = 0; i < Seats.Count; i++)
        {
            if (Seats[i].occupant == playerInstance)
            {
                Seats[i].occupant = null;
                playerInstance.ToggleControl();

                DisableModulesForSeat(i);
            }
        }

        availableSeats++;
    }

    public void OnTakeoffInitiated()
    {
        State = ShipState.takeoff;
    }

    public void OnTakeoffFinished()
    {
        State = _numEnteredAtmospheres > 0 ? ShipState.planitary_flight : ShipState.space_flight;
    }

    public void OnLandingInitiated()
    {
        State = ShipState.landing;
    }

    public void OnLandingFinished()
    {
        State = ShipState.landed;
    }

    #region Toggling Modules
    void EnableModulesForSeat(int seatPos)
    {
        if (seatPos < Seats.Count)
        {
            EnableModulesForSeat(Seats[seatPos]);
        }
    }
    void EnableModulesForSeat(ShipSeat seat)
    {
        if (seat.role == ShipSeatRole.pilot && seat.controlledModules.Count < 1) // if pilot has no pilot module
        {
            Debug.LogWarning("Attempting to pilot ship without a pilot module");
            // !!! TODO error handling here? instantiate a module?
        }
        SetModuleEnabledStaes(seat.controlledModules, true);
    }

    void DisableModulesForSeat(int seatPos)
    {
        if (seatPos < Seats.Count)
        {
            DisableModulesForSeat(Seats[seatPos]);
        }
    }
    void DisableModulesForSeat(ShipSeat seat)
    {
        SetModuleEnabledStaes(seat.controlledModules, false);
    }

    void SetModuleEnabledStaes(List<ShipModuleController> modules, bool enabledState)
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].enabled = enabledState;
        }
    }
    #endregion
}
