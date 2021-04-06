﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler animatorHandler;
    void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }
    public void HandleLightAttact(WeaponItem weapon)
    {
        animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
    }

    public void HandleHeavyAttact(WeaponItem weapon)
    {
        animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
    }
}
