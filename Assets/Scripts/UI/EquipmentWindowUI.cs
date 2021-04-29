using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWindowUI : MonoBehaviour
{
    public bool rightHandSlot01Selected;
    public bool rightHandSlot02Selected;
    public bool rightHandSlot03Selected;
    public bool leftHandSlot01Selected;
    public bool leftHandSlot02Selected;
    public bool leftHandSlot03Selected;

    HandEquipmentSlotUI[] handEquipmentSlotUIs;

    private void Awake()
    {
        handEquipmentSlotUIs = GetComponentsInChildren<HandEquipmentSlotUI>();
        gameObject.SetActive(false);
    }

    public void LoadWeaponsOnEquipmentScreen(PlayerInventory inventory)
    {
        for (int i = 0; i < handEquipmentSlotUIs.Length; i++)
        {
            if(handEquipmentSlotUIs[i].rightHandSlot01)
            {
                handEquipmentSlotUIs[i].AddItem(inventory.weaponsInRightHandSlots[0]);
            }
            else if(handEquipmentSlotUIs[i].rightHandSlot02)
            {
                 handEquipmentSlotUIs[i].AddItem(inventory.weaponsInRightHandSlots[1]);
            }
            else if(handEquipmentSlotUIs[i].rightHandSlot03)
            {
                 handEquipmentSlotUIs[i].AddItem(inventory.weaponsInRightHandSlots[2]);
            }else if (handEquipmentSlotUIs[i].leftHandSlot01)
            {
                handEquipmentSlotUIs[i].AddItem(inventory.weaponsInLeftHandSlots[0]);
            }
            else if(handEquipmentSlotUIs[i].leftHandSlot02)
            {
                 handEquipmentSlotUIs[i].AddItem(inventory.weaponsInLeftHandSlots[1]);
            }
            else if(handEquipmentSlotUIs[i].leftHandSlot03)
            {
                 handEquipmentSlotUIs[i].AddItem(inventory.weaponsInLeftHandSlots[2]);
            }

        }
    }

    public void SelectRightSlot01()
    {
        rightHandSlot01Selected = true;
    }

    public void SelectRightSlot02()
    {
        rightHandSlot02Selected = true;
    }

    public void SelectRightSlot03()
    {
        rightHandSlot03Selected = true;
    }

    public void SelectLeftSlot01()
    {
        leftHandSlot01Selected = true;
    }

    public void SelectLeftSlot02()
    {
        leftHandSlot02Selected = true;
    }

    public void SelectLeftSlot03()
    {
        leftHandSlot03Selected = true;
    }

}
