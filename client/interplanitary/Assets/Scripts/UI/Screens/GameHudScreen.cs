using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHudScreen : UIScreen
{
    public override ScreenName screenName
    {
        get
        {
            return ScreenName.GameHud;
        }
    }

    [Header("Game Hud Configuration")]
    [SerializeField]
    Transform WeaponSlotsContainer;
    [SerializeField]
    UIItemSlot WeaponSlotPrefab;

    List<UIItemSlot> WeaponSlotElements;
    int selectedSlotIndex = -1;

    public override IEnumerator Init(params object[] parameters)
    {
        GetWeaponSlotElements();

        yield return null;
    }

    void GetWeaponSlotElements()
    {
        WeaponSlotElements = new List<UIItemSlot>(WeaponSlotsContainer.GetComponentsInChildren<UIItemSlot>());
    }

    public void UpdateWeaponSlot(int slotIndex, bool isSelected, Weapon weapon = null)
    {
        if(slotIndex < 0)
        {
            Debug.LogError("[GameHudScreen : SetWeaponSlot] slotIndex cannot be negative");
            return;
        }
        else if(slotIndex > WeaponSlotElements.Count - 1)
        {
            UIItemSlot newSlot = Instantiate(WeaponSlotPrefab, WeaponSlotsContainer);
            newSlot.transform.SetAsFirstSibling();
            WeaponSlotElements.Add(newSlot);
        }

        if(weapon == null)
        {
            WeaponSlotElements[slotIndex].SetSlotProperties(isSelected);
        }
        else
        {
            WeaponSlotElements[slotIndex].SetSlotProperties(isSelected, weapon.ItemName, weapon.Icon);
        }
    }

    protected override void OnExit()
    {
    }

    protected override void OnStart()
    {
    }

    protected override void OnUpdate()
    {
    }
}
