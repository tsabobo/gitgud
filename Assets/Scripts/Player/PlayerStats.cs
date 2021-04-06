using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Vigor
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;
    AnimatorHandler animatorHandler;
    void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
    }
    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;

        HealthBar.Instance.SetMaxHealth(maxHealth);
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
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
}
