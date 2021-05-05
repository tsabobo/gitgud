using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : Singleton<WeaponSlotManager>
{
    WeaponHolderSlot leftHandSlot;
    WeaponHolderSlot rightHandSlot;
    DamageCollider leftHandDamageCollider;
    DamageCollider rightHandDamageCollider;
    Animator animator;
    PlayerStats playerStats;
    public WeaponItem attackingWeapon;
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerStats = GetComponentInParent<PlayerStats>();
        WeaponHolderSlot[] slots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (var slot in slots)
        {
            if (slot.isLeftHandSlot)
            {
                leftHandSlot = slot;
            }
            else if (slot.isRightHandSlot)
            {
                rightHandSlot = slot;
            }
        }
    }
    #region  Handle weapons damage collider
    public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
    {
        if (isLeft)
        {
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftHandDamageCollider();
            QuickSlotsUI.Instance.UpdateWeaponQuickSlotsUI(isLeft, weaponItem);
            #region  Handle Weapon Idle Animations
            if (weaponItem != null)
            {
                animator.CrossFade(weaponItem.left_Hand_Idle, 0.2f);
            }
            else
            {
                animator.CrossFade("Left Arm Empty", 0.2f);
            }
            #endregion
        }
        else
        {
            if (InputHandler.Instance.twoHandFlag)
            {
                // Move current left hand weapon to the back or disable it 
                animator.CrossFade(weaponItem.two_Handed_Idle, 0.2f);

            }
            else
            {
                #region  Handle Weapon Idle Animations
                animator.CrossFade("Both Arms Empty", 0.2f);
                if (weaponItem != null)
                {
                    animator.CrossFade(weaponItem.right_Hand_Idle, 0.2f);
                }
                else
                {
                    animator.CrossFade("Right Arm Empty", 0.2f);
                }
                #endregion
            }

            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRighttHandDamageCollider();
            QuickSlotsUI.Instance.UpdateWeaponQuickSlotsUI(isLeft, weaponItem);

        }
    }

    private void LoadLeftHandDamageCollider()
    {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    private void LoadRighttHandDamageCollider()
    {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    public void OpenRightDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void OpenLeftDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void CloseRightDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void CloseLeftDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }
    #endregion

    #region  Handle weapon Stamina Drainage
    public void DrainStaminaLightAttact()
    {
        playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
    }
    public void DrainStaminaHeavyAttact()
    {
        playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
    }
    #endregion
}
