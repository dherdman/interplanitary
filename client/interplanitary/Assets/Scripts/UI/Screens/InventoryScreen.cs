using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : UIScreen
{
    [Header("Inventory Screen Configuration")]
    [SerializeField]
    UIItemSlot itemSlotPrefab;
    [SerializeField]
    Transform equippedWeaponsContainer;
    [SerializeField]
    Transform backpackItemsContainer;

    List<UIItemSlot> backpackItemSlots;
    List<UIItemSlot> equippedWeaponSlots;

    Character CurrentCharacter;

    public override ScreenName screenName
    {
        get
        {
            return ScreenName.Inventory;
        }
    }

    public override IEnumerator Init(params object[] parameters)
    {
        if (parameters == null)
        {
            Debug.LogError("[InventoryScreen] Inventory Menu initalized with null parameters. Requires a Character object");
            yield break;
        }
        else if (parameters.Length < 1)
        {
            Debug.LogError("[InventoryScreen] Inventory Menu initalized with empty parameters. Requires a Character object");
            yield break;
        }
        else if (!parameters[0].GetType().IsSubclassOf(typeof(Character)))
        {
            Debug.LogError("[InventoryScreen] Inventory Menu initalized with incorrect parameters. First parameter must be a Character object");
            yield break;
        }

        CurrentCharacter = (Character)parameters[0];

        PopulateInventory();

        yield return null;
    }

    void PopulateInventory()
    {
        if (backpackItemSlots == null)
        {
            backpackItemSlots = new List<UIItemSlot>();
        }
        if (equippedWeaponSlots == null)
        {
            equippedWeaponSlots = new List<UIItemSlot>();
        }

        // Populate equipped weapons list
        for(int i = 0; i < CurrentCharacter.CharacterInventory.Equipped.Weapons.Count; i++)
        {
            if(i > equippedWeaponSlots.Count - 1)
            {
                equippedWeaponSlots.Add(Instantiate(itemSlotPrefab, equippedWeaponsContainer));
            }

            UpdateEquippedWeaponSlot(i);
        }

        // Populate backpack items list
        for (int i = 0; i < CurrentCharacter.CharacterInventory.Backpack.Count; i++)
        {
            if(i > backpackItemSlots.Count - 1)
            {
                backpackItemSlots.Add(Instantiate(itemSlotPrefab, backpackItemsContainer));
            }

            UpdateBackpackSlot(i);
        }
    }

    public void UpdateEquippedWeaponSlot (int i)
    {
        UpdateSlot(i, equippedWeaponSlots, CurrentCharacter.CharacterInventory.Equipped.Weapons[i], CurrentCharacter.CharacterInventory.Equipped.IsWeaponSlotSelected(i));
    }

    public void UpdateBackpackSlot (int i)
    {
        UpdateSlot(i, backpackItemSlots, CurrentCharacter.CharacterInventory.Backpack[i]);
    }

    void UpdateSlot(int i, List<UIItemSlot> slotsList, Item itm, bool selected = false)
    {
        if(itm)
        {
            slotsList[i].SetSlotProperties(selected, itm.ItemName, itm.Icon);
        }
        else
        {
            slotsList[i].SetSlotProperties(selected);
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
