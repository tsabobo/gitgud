using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    
    AnimatorHandler animatorHandler;
    void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }
    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;

        maxStamina = SetMaxStaminaFromStaminaLevel();
        currentStamina = maxStamina;

        HealthBar.Instance.SetMaxHealth(maxHealth);
        StaminaBar.Instance.SetMaxStamina(maxStamina);
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    private int SetMaxStaminaFromStaminaLevel()
    {
        maxStamina = staminaLevel * 10;
        return maxStamina;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        HealthBar.Instance.SetCurrentHealth(currentHealth);

        animatorHandler.PlayTargetAnimation(AnimatorHandler.Damage_STATE, true);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            animatorHandler.PlayTargetAnimation(AnimatorHandler.Death_STATE, true);

            // YOU DIED
        }
    }

    public void TakeStaminaDamage(int damage)
    {
        currentStamina = currentStamina - damage;
        StaminaBar.Instance.SetCurrentStamina(currentStamina);
    }
}
