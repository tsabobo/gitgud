using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    EnemyLocomotionManager enemyLocomotion;
    void Awake()
    {        
        anim = GetComponent<Animator>();
        enemyLocomotion = transform.parent.GetComponent<EnemyLocomotionManager>();
    }

    void OnAnimatorMove()
    {
        float delta = Time.deltaTime;
        enemyLocomotion.enemyRigidbody.drag = 0;

        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        enemyLocomotion.enemyRigidbody.velocity = velocity ;
    }
}
