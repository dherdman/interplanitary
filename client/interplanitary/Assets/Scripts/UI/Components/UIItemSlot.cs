using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetSlotProperties(bool selected, string _itemName = "", Sprite _itemIcon = null)
    {
        itemName.text = _itemName;
        if(_itemIcon != null) // if a new icon is given, set sprite and ensure white color
        {
            itemIcon.color = Color.white;
            itemIcon.sprite = _itemIcon;
        }
        else if(itemIcon.sprite == null) // if no new icon is given and there is no existing icon, set icon to clear
        {
            itemIcon.color = Color.clear;
        }

        selectedBackdrop.SetActive(selected);
        unselectedBackdrop.SetActive(!selected);
    }

}
