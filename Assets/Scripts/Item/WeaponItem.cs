﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item
{
    public GameObject modelPrefab;
    public bool isUnarmed;
    [Header("Idle Animations")]
    public string right_Hand_Idle;
    public string left_Hand_Idle;
    public string two_Handed_Idle;
    [Header("One Handed Attack Animations")]
    public string OH_Light_Attack_01;
    public string OH_Light_Attack_02;
    public string OH_Heavy_Attack_01;
    public string OH_Heavy_Attack_02;
    [Header("Tow Handed Attack Animations")]
    public string TH_Light_Attack_01;
    public string TH_Heavy_Attack_01;
    public string TH_Light_Attack_02;
    public string TH_Heavy_Attack_02;
    [Header("Stamina Costs")]
    public int baseStamina;
    public float lightAttackMultiplier;
    public float heavyAttackMultiplier;


}
