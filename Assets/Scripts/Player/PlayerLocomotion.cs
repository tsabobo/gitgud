using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Transform cameraObject;
    InputHandler inputHandler;
    public Vector3 moveDirection;
    PlayerManager playerManager;

    [HideInInspector]
    public Transform myTransform;
    [HideInInspector]
    public AnimatorHandler animatorHandler;
    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    [Header("Ground & Air Dtection Stats")]
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    [Tooltip("Height between Capsule Collider bottom to ground")]
    float groundDetectionRayDistance = 0.2f;
    public LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [Header("Movement Stats")]
    [SerializeField]
    float walkingSpeed = 1f;
    [SerializeField]
    float runningSpeed = 5;
    [SerializeField]
    float sprintSpeed = 7;
    [SerializeField]
    float rotationSpeed = 10;
    [SerializeField]
    float fallingingSpeed = 45;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        myTransform = transform;

        animatorHandler.Initialize();

        playerManager.isGrounded = true;
        // Ignore some layers
        ignoreForGroundCheck = ~(1 << 8 | 1 << 11 | 1 << 12);
    }

    #region Movement

    Vector3 normalVector;
    Vector3 targetPosition;

    private void HandleRotation(float delta)
    {
        Vector3 targetDir = Vector3.zero;
        float moveOverride = inputHandler.moveAmount;

        targetDir = cameraObject.forward * inputHandler.vertical;
        targetDir += cameraObject.right * inputHandler.horizontal;

        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
            targetDir = myTransform.forward;

        float rs = rotationSpeed;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
        myTransform.rotation = targetRotation;
    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag)
            return;

        if (playerManager.isInteracting)
            return;

        moveDirection = cameraObject.forward * inputHandler.vertical;
        moveDirection += cameraObject.right * inputHandler.horizontal;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // When left stick is slightly tilt, character walks
        float speed = walkingSpeed;
        if (inputHandler.moveAmount >= 0.5f)
        {
            // When left stick is pushed further, character runs
            speed = runningSpeed;
        }

        // When sprint button is pressed, character runs
        if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
        {
            speed = sprintSpeed;
            playerManager.isSprinting = true;
            moveDirection *= speed;
        }
        else
        {
            if (inputHandler.moveAmount < 0.5f)
            {
                moveDirection *= walkingSpeed;
                 playerManager.isSprinting = false;
            }
            else
            {
                moveDirection *= speed;
                 playerManager.isSprinting = false;
            }            
        }


        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity;
        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandleRollingAndSrinting(float delta)
    {
        if (animatorHandler.anim.GetBool("isInteracting"))
            return;
        if (inputHandler.rollFlag)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;

            if (inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation(AnimatorHandler.Rolling_STATE, true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
            else
            {
                animatorHandler.PlayTargetAnimation(AnimatorHandler.Backstep_STATE, true);
            }
        }
    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        // if something is in front of you, you are not moveing
        if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if (playerManager.isInAir)
        {
            rigidbody.AddForce(-Vector3.up * fallingingSpeed);

            // Hopping a little bit off edge with a small force
            rigidbody.AddForce(moveDirection * fallingingSpeed / 5f);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDetectionRayDistance;

        targetPosition = myTransform.position;
        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);
        if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            // Hit something, we are landing
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;

            // Landing Animation
            if (playerManager.isInAir)
            {                
                if (inAirTimer > 0.5f)
                {
                    animatorHandler.PlayTargetAnimation(AnimatorHandler.Land_STATE, true);
                    inAirTimer = 0;
                }
                else
                {
                    // Player falls very shortly, not need to play landing animation
                    animatorHandler.PlayTargetAnimation(AnimatorHandler.Empty_STATE, false);
                     inAirTimer = 0;
                }

                playerManager.isInAir = false;
            }
        }
        else
        {
            // Raycast hit nothing, we are falling
            if (playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if (playerManager.isInAir == false)
            {
                if (playerManager.isInteracting == false)
                {
                    animatorHandler.PlayTargetAnimation(AnimatorHandler.Falling_STATE, true);
                }

                Vector3 velocity = rigidbody.velocity;
                velocity.Normalize();
                // TODO: check here if we were running or walking before
                rigidbody.velocity = velocity * (runningSpeed / 2);
                playerManager.isInAir = true;
            }
            
            // TODO: fix bug that player get stuck at egde on stairs
            #region  dirty fix
            else
            {
                bool isFallState = animatorHandler.CheckCurrentAnimationState(AnimatorHandler.Falling_STATE);

                if (inAirTimer > 0.5f && isFallState)
                {
                    print("dirty fix");
                    // Move forward a big to get out of the stuck and keep falling
                    rigidbody.velocity = myTransform.forward * (walkingSpeed / 2);
                    animatorHandler.PlayTargetAnimation(AnimatorHandler.Falling_STATE, false);
                    inAirTimer = 0;
                }
                
            }
            #endregion;  
        }
        if(playerManager.isInteracting || inputHandler.moveAmount > 0)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
        }
        else
        {
            myTransform.position = targetPosition;
        }

        if (playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputHandler.moveAmount > 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }
    }
    public void HandleJumping()
    {
        if(playerManager.isInteracting)
            return;
        
        if(inputHandler.jump_Input)
        {
            if(inputHandler.moveAmount > 0)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
                animatorHandler.PlayTargetAnimation(AnimatorHandler.Jump_STATE, true);
                moveDirection.y = 0;
                Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = jumpRotation;
            }
        }
    }
    #endregion

}
