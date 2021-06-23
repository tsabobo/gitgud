using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLocomotionManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public EnemyAnimatorManager enemyAnimator;
    public NavMeshAgent naveMeshAgent;
    public Rigidbody enemyRigidbody;

    public LayerMask detectionLayer;
    public CharacterStats currentTarget;
    public float distanceFromTarget;
    public float stoppingDistance = 1f;
    public float rotationSpeed = 15f;
    public void Awake()
    {
        naveMeshAgent = GetComponentInChildren<NavMeshAgent>();
        enemyRigidbody = GetComponent<Rigidbody>();
    }

    public void Start()
    {
        naveMeshAgent.enabled = false;
        enemyRigidbody.isKinematic = false;
    }

    public void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

            if (characterStats != null)
            {
                // Check for team ID

                Vector3 targetDirection = characterStats.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > enemyManager.minimumDirectionAngle && viewableAngle < enemyManager.maxDirectionAngle)
                {
                    currentTarget = characterStats;
                }
            }
        }
    }

    public void HandleMoveToTarget()
    {
        if(enemyManager.isPerformingAction)
            return;
            
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
        float viewableAngle = Vector3.Angle(targetDirection, transform.forward);


        // If performing, stop movement
        if (enemyManager.isPerformingAction)
        {
            enemyAnimator.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            naveMeshAgent.enabled = false;
        }
        else
        {
            if (distanceFromTarget > stoppingDistance)
            {
                enemyAnimator.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                enemyAnimator.anim.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }
        }

        HandleRotationTowardTarget();
        naveMeshAgent.transform.localPosition = Vector3.zero;
        naveMeshAgent.transform.localRotation = Quaternion.identity;
    }

    public void HandleRotationTowardTarget()
    {
        // Rotate manually
        if (enemyManager.isPerformingAction)
        {
            Vector3 direction = currentTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            if (direction == Vector3.zero)
            {
                direction = transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed / Time.deltaTime);
        }
        // Rotate with pathfinding 
        else
        {
            Vector3 relativeDirection = transform.InverseTransformDirection(naveMeshAgent.desiredVelocity);
            Vector3 targetVelocity = enemyRigidbody.velocity;

            naveMeshAgent.enabled = true;
            naveMeshAgent.SetDestination(currentTarget.transform.position);

            enemyRigidbody.velocity = targetVelocity;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, naveMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
        }
    }
}
