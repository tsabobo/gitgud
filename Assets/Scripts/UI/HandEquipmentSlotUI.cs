using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandEquipmentSlotUI : MonoBehaviour
{
    public Image icon;
    public WeaponItem weapon; 
    public bool rightHandSlot01;
    public bool rightHandSlot02;
    public bool rightHandSlot03;
    public bool leftHandSlot01;
    public bool leftHandSlot02;
    public bool leftHandSlot03;

    public void AddItem(WeaponItem newWeapon)
    {
        weapon = newWeapon;
        icon.sprite = weapon.itemIcon;
        icon.enabled = true;
        icon.gameObject.SetActive(true);
    }

    public void ClearItem()
    {
        weapon = null;
        icon.sprite = null;
        icon.enabled = false;
        icon.gameObject.SetActive(false);
    }

    public void SelectThisSlot()
    {
        if(rightHandSlot01)
        {
            UIManager.Instance.rightHandlSolot01Selected = true;
        }
        else if (rightHandSlot02)
        {
            UIManager.Instance.rightHandlSolot02Selected = true;
        }
        else if (rightHandSlot03)
        {
            UIManager.Instance.rightHandlSolot03Selected = true;
        }
        else if (leftHandSlot01)
        {
            UIManager.Instance.leftHandlSolot01Selected = true;
        }
        else if (leftHandSlot02)
        {
            UIManager.Instance.leftHandlSolot02Selected = true;
        }
        else if (leftHandSlot03)
        {
            UIManager.Instance.leftHandlSolot03Selected = true;
        }
    }
}
