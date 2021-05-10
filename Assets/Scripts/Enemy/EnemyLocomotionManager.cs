using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocomotionManager : MonoBehaviour
{
    public EnemyManager enemyManager;
    public LayerMask detectionLayer;
    public CharacterStats currentTarget;
     public void HandleDetection()
     {
         Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);
         for (int i = 0; i < colliders.Length; i++)
         {
             CharacterStats characterStats = colliders[i].transform.GetComponent<CharacterStats>();

             if(characterStats != null)
             {
                 // Check for team ID

                 Vector3 targetDirection = characterStats.transform.position - transform.position;
                 float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
                 
                 if( viewableAngle > enemyManager.minimumDirectionAngle && viewableAngle < enemyManager.maxDirectionAngle)
                 {
                     currentTarget = characterStats;
                 }
             }
         }
     }
}
