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
    public List<ShipModule> controlledModules;
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

public class Ship : MonoBehaviour, IInteractable
{
    [SerializeField]
    Vector3 cameraPosition;

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

    int availableSeats;

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

    void Start()
    {
        if (Seats.Count < 1)
        {
            GameObject moduleHolder = new GameObject();
            moduleHolder.transform.parent = transform;
            moduleHolder.transform.position = Vector3.zero;
            CockpitModule module = moduleHolder.AddComponent<CockpitModule>();
            module.InitializeModule(this);

            Seats = new List<ShipSeat> {
                new ShipSeat
                {
                    occupant = null,
                    role = ShipSeatRole.pilot,
                    controlledModules = new List<ShipModule> { module },
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
                        if (Seats[i].controlledModules[j] == null || Seats[i].controlledModules[j].GetType() == typeof(CockpitModule))
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

        EnableModulesForSeat(playerSeat);
        playerInstance.ToggleControl();

        CameraManager.instance.AssignPlayerCameraToTarget(transform, cameraPosition);

        availableSeats--;
    }

    void Disembark(Player playerInstance)
    {
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

    void SetModuleEnabledStaes(List<ShipModule> modules, bool enabledState)
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].enabled = enabledState;
        }
    }
    #endregion
}
