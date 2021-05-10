using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    Animator animator;
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;

        animator.Play(AnimatorHandler.Damage_STATE);

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            animator.Play(AnimatorHandler.Death_STATE);

            // Handle death
            if (currentHealth <= 0)
                gameObject.tag = "DeadEnemy";
        }
    }
}
