using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler animatorHandler;
    InputHandler inputHandler;
    WeaponSlotManager weaponSlotManager;
    public string lastAttack;

    void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        inputHandler = GetComponentInChildren<InputHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);
            if (lastAttack == weapon.OH_Light_Attack_01)
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_02, true);
            }

        if (lastAttack == weapon.OH_Heavy_Attack_01)
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_02, true);
            }
        }
    }
    public void HandleLightAttact(WeaponItem weapon)
    {
        weaponSlotManager.attackingWeapon = weapon;
        animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_01, true);
        lastAttack = weapon.OH_Light_Attack_01;
    }

    public void HandleHeavyAttact(WeaponItem weapon)
    {
        weaponSlotManager.attackingWeapon = weapon;
        animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_01, true);
        lastAttack = weapon.OH_Heavy_Attack_01;
    }
}
