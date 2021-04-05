using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot;
    WeaponHolderSlot rightHandSlot;

    void Awake()
    {
        WeaponHolderSlot[] slots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (var slot in slots)
        {
            if(slot.isLeftHandSlot)
            {
                leftHandSlot = slot;
            }
            else if(slot.isRightHandSlot)
            {
                rightHandSlot = slot;
            }
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if(isLeft)
        {
            leftHandSlot.LoadWeaponModel(weaponItem);
        }
        else
        {
            rightHandSlot.LoadWeaponModel(weaponItem);
        }
    }
}
