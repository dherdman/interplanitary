using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScreen : UIScreen
{
    class ActiveSlot
    {
        public int SlotIndex;
        public UIItemSlot SlotInstance;
        public bool IsEquipped;

        public ActiveSlot(int _idx, UIItemSlot _slotInstance, bool _isEquipped)
        {
            SlotIndex = _idx;
            SlotInstance = _slotInstance;
            IsEquipped = _isEquipped;
        }
    }

    [Header("Inventory Screen Configuration")]
    [SerializeField]
    GameObject itemSlotPrefab;
    [SerializeField]
    Transform equippedWeaponsContainer;
    [SerializeField]
    Transform backpackItemsContainer;

    List<UIItemSlot> backpackItemSlots;
    List<UIItemSlot> equippedWeaponSlots;

    Character CurrentCharacter;

    ActiveSlot activeSlot = null;

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
        else if (!(parameters[0] is Character))
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
                GameObject slotObj = Instantiate(itemSlotPrefab, equippedWeaponsContainer);
                UIItemSlot slotInstance = slotObj.GetComponent<UIItemSlot>();
                equippedWeaponSlots.Add(slotInstance);

                int captured_i = i;
                slotObj.GetComponent<UIButton>().onClick.AddListener(delegate { OnItemSlotClick(captured_i, slotInstance, true); });
            }

            UpdateEquippedSlot(i);
        }

        // Populate backpack items list
        for (int i = 0; i < CurrentCharacter.CharacterInventory.Backpack.Count; i++)
        {
            if(i > backpackItemSlots.Count - 1)
            {
                GameObject slotObj = Instantiate(itemSlotPrefab, backpackItemsContainer);
                UIItemSlot slotInstance = slotObj.GetComponent<UIItemSlot>();
                backpackItemSlots.Add(slotInstance);

                int captured_i = i;
                slotObj.GetComponent<UIButton>().onClick.AddListener(delegate { OnItemSlotClick(captured_i, slotInstance, false); });
            }

            UpdateBackpackSlot(i);
        }
    }

    void OnItemSlotClick (int slotIdx, UIItemSlot slotInstance, bool isEquipped)
    {
        if (activeSlot == null)
        {
            activeSlot = new ActiveSlot(slotIdx, slotInstance, isEquipped);

            slotInstance.SetSelected(true);
        }
        else
        {
            // ask character to update their inventory state
            if(CurrentCharacter.SwapItems(activeSlot.SlotIndex, activeSlot.IsEquipped, slotIdx, isEquipped))
            {
                UpdateSlot(activeSlot.SlotIndex, activeSlot.IsEquipped);
                UpdateSlot(slotIdx, isEquipped);
            }

            activeSlot.SlotInstance.SetSelected(false);
            activeSlot = null;
        }
    }

    public void UpdateSlot(int i, bool equipped)
    {
        if(equipped)
        {
            UpdateEquippedSlot(i);
        }
        else
        {
            UpdateBackpackSlot(i);
        }
    }

    void UpdateEquippedSlot (int i)
    {
        UpdateSlotWithListItem(i, equippedWeaponSlots, CurrentCharacter.CharacterInventory.Equipped.Weapons[i], CurrentCharacter.CharacterInventory.Equipped.IsWeaponSlotSelected(i));
    }

    void UpdateBackpackSlot (int i)
    {
        UpdateSlotWithListItem(i, backpackItemSlots, CurrentCharacter.CharacterInventory.Backpack[i]);
    }

    void UpdateSlotWithListItem(int i, List<UIItemSlot> slotsList, Item itm, bool selected = false)
    {
        if(itm)
        {
            slotsList[i].SetSlotProperties(selected, itm.ItemName, itm.Icon);
        }
        else
        {
            slotsList[i].SetSlotProperties(selected, "", null);
        }
    }

    protected override void OnStart()
    {
    }

    protected override void OnExit()
    {
        activeSlot = null;
    }

    protected override void OnUpdate()
    {
    }
}
