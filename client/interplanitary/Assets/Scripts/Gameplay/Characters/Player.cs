using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    Transform WeaponHold;

    [SerializeField]
    Weapon EquippedWeaponPrefab;
    public Weapon EquippedWeapon
    {
        get; private set;
    }

    PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if(EquippedWeaponPrefab)
        {
            EquippedWeapon = Instantiate(EquippedWeaponPrefab, WeaponHold.position, WeaponHold.rotation) as Weapon;
            EquippedWeapon.transform.parent = WeaponHold;
        }
    }

    void Update()
    {

    }

    /// <summary>
    /// Toggles this player's controller enabled/disabled
    /// </summary>
    public void ToggleControl()
    {
        if (playerController != null)
        {
            if (playerController.enabled)
            {
                playerController.DisableSelf();
            }
            else
            {
                playerController.enabled = true;
            }
        }
    }

    // !!!!! TODO(also in player controller) should be animation based, this is temp
    public void AimAt(Vector3 aimTarget)
    {
        // only guns can be aimed, melee does not change based on mouse position
        if(EquippedWeapon != null && EquippedWeapon.GetType() == typeof(Gun))
        {
            WeaponHold.LookAt(aimTarget);
            WeaponHold.localRotation = Quaternion.Euler(WeaponHold.localRotation.eulerAngles.x, WeaponHold.localRotation.eulerAngles.y, 0f);
        }
    }

    public void FireWeapon()
    {
        if(EquippedWeapon != null)
        {
            EquippedWeapon.Fire();
        }
    }

    public void PickupItem(Item item)
    {

    }
}
