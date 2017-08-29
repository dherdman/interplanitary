using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [Header("Gun Tuning")]
    [SerializeField]
    protected float Range;
    [SerializeField]
    protected float MuzzleVelocity;

    [Space]
    [SerializeField]
    protected float RateOfFire;
    [SerializeField]
    protected int MagSize;
    [SerializeField]
    protected int AmmoPerShot;
    [SerializeField]
    protected AmmoType ammoType;

    [Space]
    [SerializeField]
    Projectile BulletPrefab;

    GunModel OwnGunModelInstance;

    protected int NumBulletsLoaded;
    float nextShotTime;
    float minShotDelay;

    void Start()
    {
        NumBulletsLoaded = MagSize; // !!! TODO initialize w/ 0 bullets once inventory is ready

        SetShotInterval();

        nextShotTime = 0f;

        if(ItemModelInstance.GetType() != typeof(GunModel))
        {
            Debug.LogError("[Gun] Gun's model is not of type GunModel");
        }
        else
        {
            OwnGunModelInstance = (GunModel)ItemModelInstance;
        }
    }

    public override void Reload()
    {
        if (!IsFullyLoaded())
        {
            PerformReload();
        }
    }

    void PerformReload()
    {
        // TODO animation before refilling ammo
        NumBulletsLoaded = MagSize;
    }

    public override void Fire()
    {
        if (CanFire())
        {
            PerformFire();
        }
        else
        {
            Reload();
        }
    }

    void PerformFire()
    {
#if UNITY_EDITOR
        SetShotInterval();
#endif
        nextShotTime = Time.time + minShotDelay;

        Projectile bullet = Instantiate(BulletPrefab, OwnGunModelInstance.Muzzle.position, Quaternion.Euler(OwnGunModelInstance.Muzzle.rotation.eulerAngles.x, OwnGunModelInstance.Muzzle.rotation.eulerAngles.y, 0f), ProjectileContainer.instance.transform) as Projectile;
        bullet.Init(Damage, MuzzleVelocity, Range);

        NumBulletsLoaded -= AmmoPerShot;
        if (NumBulletsLoaded < 0)
        {
            NumBulletsLoaded = 0;
        }
    }

    bool CanFire()
    {
        return IsLoaded() && Time.time > nextShotTime;
    }

    bool IsLoaded()
    {
        // a zero mag size is considered loaded w/ 0 loaded bullets, otherwise only if num loaded > 0
        return IsFullyLoaded() || NumBulletsLoaded > 0;
    }

    bool IsFullyLoaded()
    {
        return NumBulletsLoaded == MagSize;
    }

    void SetShotInterval()
    {
        if (RateOfFire != 0)
        {
            minShotDelay = 1 / RateOfFire;
        }
        else
        {
            minShotDelay = float.MaxValue;
        }
    }
}
