using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventorySlot : MonoBehaviour
{
    public Image icon;
    WeaponItem item;

    public void AddItem(WeaponItem newItem)
    {
        item = newItem;
        icon.sprite = item.itemIcon;
        icon.enabled = true;
        gameObject.SetActive(true);
    }

    public void ClearInventorySlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        gameObject.SetActive(false);
    }

    public void EquipThisItem()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        UIManager ui = UIManager.Instance;

        if (ui.rightHandlSolot01Selected)
        {
            // Add current item to inventory
            inventory.weaponInventory.Add(inventory.weaponsInRightHandSlots[0]);
            // Equip this
            inventory.weaponsInRightHandSlots[0] = item;
        }
        else if (ui.rightHandlSolot02Selected)
        {
            inventory.weaponInventory.Add(inventory.weaponsInRightHandSlots[1]);
            inventory.weaponsInRightHandSlots[1] = item;
        }
        else if (ui.rightHandlSolot03Selected)
        {
            inventory.weaponInventory.Add(inventory.weaponsInRightHandSlots[2]);
            inventory.weaponsInRightHandSlots[2] = item;
        }
        else if (ui.leftHandlSolot01Selected)
        {
            inventory.weaponInventory.Add(inventory.weaponsInLeftHandSlots[0]);
            inventory.weaponsInLeftHandSlots[0] = item;
        }
        else if (ui.leftHandlSolot02Selected)
        {
            inventory.weaponInventory.Add(inventory.weaponsInLeftHandSlots[1]);
            inventory.weaponsInLeftHandSlots[1] = item;
        }
        else if (ui.leftHandlSolot03Selected)
        {
            inventory.weaponInventory.Add(inventory.weaponsInLeftHandSlots[2]);
            inventory.weaponsInLeftHandSlots[2] = item;
        }
        // Remove current item
        inventory.weaponInventory.Remove(item);

        inventory.rightWeapon = inventory.weaponsInRightHandSlots[inventory.currentRightWeaponIndex];
        inventory.leftWeapon = inventory.weaponsInLeftHandSlots[inventory.currentLeftWeaponIndex];

        WeaponSlotManager.Instance.LoadWeaponOnSlot(inventory.rightWeapon, false);
        WeaponSlotManager.Instance.LoadWeaponOnSlot(inventory.leftWeapon, true);

        ui.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(inventory);
        ui.ResetAllSelectedSlots();


    }
}
