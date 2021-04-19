using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;
    public InteractableUI interactableUI;
    public GameObject interactableUIObject;
    public GameObject itemInteractableObject;

    [Header("Player Flags")]
    public bool isSprinting;
    public bool isInteracting;
    public bool isInAir;
    public bool isGrounded;
    public bool canDoCombo;
    private void Awake()
    {
        cameraHandler = CameraHandler.Instance;
    }

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        interactableUI = FindObjectOfType<InteractableUI>();
        interactableUIObject.SetActive(false);
        itemInteractableObject.SetActive(false);
    }

    void Update()
    {
        // Tick
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");

        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSrinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);

        CheckForInteractable();
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        if (cameraHandler != null)
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
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;
        inputHandler.a_Input = false;
        inputHandler.d_Pad_Left = false;
        inputHandler.d_Pad_Right = false;
        inputHandler.d_Pad_Up = false;
        inputHandler.d_Pad_Down = false;

        if (isInAir)
        {
            playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
        }
    }

    public void CheckForInteractable()
    {
        RaycastHit hit;
        
        if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out hit, 1f, cameraHandler.ignoreLayers))
        {
            Debug.DrawRay(transform.position, transform.forward * 0.3f, Color.red, 0.1f);

            if (hit.collider.tag == "Interactable")
            {
                Interactable interactableObj = hit.collider.GetComponent<Interactable>();

                if (interactableObj != null)
                {
                    string interactableText = interactableObj.interactableText;
                    interactableUI.interactableText.text = interactableText;
                    interactableUIObject.SetActive(true);

                    if (inputHandler.a_Input)
                    {
                        hit.collider.GetComponent<Interactable>().Interact(this);

                    }
                }
            }
        }
        else
        {
            interactableUIObject.SetActive(false);

            if(inputHandler.a_Input)
            {
                itemInteractableObject.SetActive(false);
            }
        }
    }
}
