using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    public EnemyLocomotionManager enemyLocomotion;
    public EnemyAnimatorManager enemyAnimatorManager;
    public EnemyAttackAction[] enemyAttacks;
    public EnemyAttackAction currentAttack;
    public bool isPerformingAction;
    [Header("AI Settings")]
    public float detectionRadius = 20;
    // Enemy FOV
    public float minimumDirectionAngle = 50;
    public float maxDirectionAngle = -50;
    public float currentRecoveryTime = 0;
    void Awake()
    {
        enemyLocomotion = GetComponent<EnemyLocomotionManager>();
        enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
    }

    void Update()
    {
        HandleRecoveryTimer();
    }

    void FixedUpdate()
    {
        HandleCurrentAction();
    }

    private void HandleCurrentAction()
    {
        if (enemyLocomotion.currentTarget != null)
        {
            enemyLocomotion.distanceFromTarget = Vector3.Distance(enemyLocomotion.currentTarget.transform.position, transform.position);

        }

        if (enemyLocomotion.currentTarget == null)
        {
            enemyLocomotion.HandleDetection();
        }
        else if (enemyLocomotion.distanceFromTarget > enemyLocomotion.stoppingDistance)
        {
            enemyLocomotion.HandleMoveToTarget();
        }
        else if (enemyLocomotion.distanceFromTarget <= enemyLocomotion.stoppingDistance)
        {
            // HANDLE ATTACKS
            AttackTarget();
        }
    }
    private void HandleRecoveryTimer()
    {
        if (currentRecoveryTime > 0)
        {
            currentRecoveryTime -= Time.deltaTime;
        }

        if (isPerformingAction)
        {
            if (currentRecoveryTime < 0)
            {
                isPerformingAction = false;
            }
        }
    }

    #region Attacks
    private void GetNewAttack()
    {
        Vector3 targetsDirection = enemyLocomotion.currentTarget.transform.position - transform.position;
        float viewableAngle = Vector3.Angle(targetsDirection, transform.forward);
        enemyLocomotion.distanceFromTarget = Vector3.Distance(enemyLocomotion.currentTarget.transform.position, transform.position);

        int maxScore = 0;
        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];
            if (enemyLocomotion.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                && enemyLocomotion.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    maxScore += enemyAttackAction.attackScore;
                }
            }
        }

        int randomValue = Random.Range(0, maxScore);
        int tempScore = 0;
        for (int i = 0; i < enemyAttacks.Length; i++)
        {
            EnemyAttackAction enemyAttackAction = enemyAttacks[i];
            if (enemyLocomotion.distanceFromTarget <= enemyAttackAction.maximumDistanceNeededToAttack
                && enemyLocomotion.distanceFromTarget >= enemyAttackAction.minimumDistanceNeededToAttack)
            {
                if (viewableAngle <= enemyAttackAction.maximumAttackAngle
                && viewableAngle >= enemyAttackAction.minimumAttackAngle)
                {
                    if (currentAttack != null)
                        return;
                    tempScore += enemyAttackAction.attackScore;

                    if (tempScore > randomValue)
                    {
                        currentAttack = enemyAttackAction;
                    }
                }
            }
        }
    }

    private void AttackTarget()
    {
        if (isPerformingAction)
            return;

        if (currentAttack == null)
            GetNewAttack();
        else
        {
            isPerformingAction = true;
            currentRecoveryTime = currentAttack.recoveryTime;
            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
            currentAttack = null;
        }
    }
    #endregion
}
