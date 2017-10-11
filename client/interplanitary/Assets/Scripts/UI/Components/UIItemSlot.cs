using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIButton))]
public class UIItemSlot : MonoBehaviour
{
    [SerializeField]
    Text itemName;
    [SerializeField]
    Image itemIcon;

    [SerializeField]
    GameObject selectedBackdrop;
    [SerializeField]
    GameObject unselectedBackdrop;

    public void SetSlotProperties(bool selected, string _itemName, Sprite _itemIcon)
    {
        itemName.text = _itemName;
        itemIcon.sprite = _itemIcon;

        itemIcon.color = itemIcon.sprite != null ? Color.white : Color.clear;

        SetSelected(selected);
    }

    // !!! TODO extend to allow multiple selected states (e.g. selected weapon vs clicked in inventory vs none etc) 
    public void SetSelected(bool selected)
    {
        selectedBackdrop.SetActive(selected);
        unselectedBackdrop.SetActive(!selected);
    }
}
