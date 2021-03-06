using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : Singleton<CameraHandler>
{
    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public LayerMask ignoreLayers;
    public LayerMask environmentLayer;
    public static CameraHandler singleton;
    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.1f;
    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35f;
    public float maixmumPivot = 35f;
    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;
    public float lockedPivotPosition = 2.25f;
    public float unlockedPivotPosition = 1.65f;
    public float maxLockOnDistance = 30f;
    public PlayerManager playerManager;
    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public Transform currentLockOnTarget;
    public Transform nearestLockOnTarget;
    public Transform leftLockOnTarget;
    public Transform rightLockOnTarget;
    private void Awake()
    {
        //singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        environmentLayer = LayerMask.NameToLayer("Environment");

        // Force find player target to fix camera not following bug
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta)
    {
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position,
                                                    targetTransform.position,
                                                    ref cameraFollowVelocity,
                                                    delta / followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        if (InputHandler.Instance.lockOnFlag == false && currentLockOnTarget == null)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maixmumPivot);

            Vector3 rotation = Vector3.zero;

            rotation.y = lookAngle;
            myTransform.rotation = Quaternion.Euler(rotation);

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            cameraPivotTransform.localRotation = Quaternion.Euler(rotation);
        }
        else
        {
            float velocity = 0;

            Vector3 dir = currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;

            dir = currentLockOnTarget.position - cameraPivotTransform.position;
            dir.Normalize();
            dir.y = 0;
            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            cameraPivotTransform.localEulerAngles = eulerAngle;

        }


    }

    private void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction,
                            out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = cameraTransformPosition;

    }

    public void HanldeLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();
            if (character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                RaycastHit hit;

                if (character.transform.root != targetTransform.root 
                    && viewableAngle > -50 && viewableAngle < 50
                    && distanceFromTarget <= maxLockOnDistance)
                {
                    if (Physics.Linecast(playerManager.lockOnTransform.position,character.lockOnTransform.position, out hit ))
                    {
                        Debug.DrawLine(playerManager.lockOnTransform.position,character.lockOnTransform.position);
                        if (hit.transform.gameObject.layer == environmentLayer)
                        {
                            // Cannot lock onto target, object in the way
                        }
                        else
                        {
                            availableTargets.Add(character);
                        }
                    }
                    
                }
            }
        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[i].transform.position);
            if (distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTargets[i].lockOnTransform;
            }
            if(InputHandler.Instance.lockOnFlag)
            {
                Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[i].transform.position);
                var distanceFromLeftTarget = currentLockOnTarget.position.x - availableTargets[i].transform.position.x;
                var distanceFromRightTarget = currentLockOnTarget.position.x + availableTargets[i].transform.position.x;

                if(relativeEnemyPosition.x > 0 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockOnTarget = availableTargets[i].lockOnTransform;
                }

                if(relativeEnemyPosition.x < 0 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockOnTarget = availableTargets[i].lockOnTransform;
                }
            }
        }
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();

        currentLockOnTarget = null;
        nearestLockOnTarget = null;
    }

    public void SetCameraHeight()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
        Vector3 newUnLockedPosition = new Vector3(0, unlockedPivotPosition);
        
        // Locked on
        if(currentLockOnTarget != null)
        {
            // Smooth damp to locked height positoin
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
        }
        else
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnLockedPosition, ref velocity, Time.deltaTime);
        }

    }
}
