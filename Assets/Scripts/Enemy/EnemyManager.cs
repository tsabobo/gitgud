using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    public EnemyLocomotionManager enemyLocomotion;
    public EnemyAnimatorManager enemyAnimatorManager;
    public bool isPerformingAction;
    [Header("AI Settings")]
    public float detectionRadius = 20;
    // Enemy FOV
    public float minimumDirectionAngle = 50;
    public float maxDirectionAngle = -50;

    void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (enemyLocomotion.currentTarget == null)
        {
            enemyLocomotion.HandleDetection();
        }
        else
        {
            enemyLocomotion.HandleMoveToTarget();
        }
    }
}
