using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;


    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInteracting;
    public bool isInAir;
    public bool isGrounded;
    private void Awake()
    {
        cameraHandler = CameraHandler.Instance;
    }

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        // Tick
        float delta = Time.deltaTime; 

        isInteracting = anim.GetBool("isInteracting");   

        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSrinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        if(cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

    // Reset flags
    void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        isSprinting = inputHandler.b_Input;

        if(isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }
}
