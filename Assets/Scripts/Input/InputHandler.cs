using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;
    #region Controller Input flat
    // XBOX controller:  Y
    //                 X   B
    //                   A  
    // B button (east button on gamepad) is pressed 
    public bool b_Input;
    // A button (south button on gamepad) is pressed 
    public bool a_Input;
    // RB button (R1)
    public bool rb_Input;
    // RT button (R2)
    public bool rt_Input;
    public bool jump_Input;
    public bool inventory_Input;
    public bool d_Pad_Up;
    public bool d_Pad_Down;
    public bool d_Pad_Left;
    public bool d_Pad_Right;
    #endregion
    public bool sprintFlag;
    public bool comboFlag;
    public bool inventoryFlag;
    public bool rollFlag;
    public float rollInputTimer;
    PlayerControls inputActions;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    PlayerManager playerManager;
    Vector2 movementInput;
    Vector2 cameraInput;

    void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
        }

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
        HandleAttackInput(delta);
        HandleQuickSlotsInput();
        HandleInteractingButtonInput();
        HandleJumpInput();
        HandleInventoryInput();
    }

    private void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void HandleRollInput(float delta)
    {
        b_Input = inputActions.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
        if (b_Input)
        {
            rollInputTimer += delta;
            sprintFlag = true;
        }
        else
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.5f)
            {
                sprintFlag = false;
                rollFlag = true;
            }
            rollInputTimer = 0;
        }
    }

    private void HandleAttackInput(float delta)
    {
        inputActions.PlayerAction.RB.performed += i => rb_Input = true;
        inputActions.PlayerAction.RT.performed += i => rt_Input = true;

        if (rb_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                playerAttacker.HandleLightAttact(playerInventory.rightWeapon);
            }
        }

        if (rt_Input)
        {
            if (playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            playerAttacker.HandleHeavyAttact(playerInventory.rightWeapon);
        }
    }

    private void HandleQuickSlotsInput()
    {
        inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
        inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;

        if (d_Pad_Right)
        {
            playerInventory.ChangeRightHandWeapon();
        }
        else if (d_Pad_Left)
        {
            playerInventory.ChangeLeftHandWeapon();
        }
    }

    private void HandleInteractingButtonInput()
    {
        inputActions.PlayerAction.A.performed += i => a_Input = true;
    }

    private void HandleJumpInput()
    {
        inputActions.PlayerAction.Jump.performed += i => jump_Input = true;
    }

    private void HandleInventoryInput()
    {
        inputActions.PlayerAction.Inventory.performed += i => inventory_Input = true;
        if (inventory_Input)
        {
            inventoryFlag = !inventoryFlag;

            if (inventoryFlag)
            {
                UIManager.Instance.OpenSelectWindow();
                UIManager.Instance.UpdateUI();
                // Close HUD when inventory window is opened and vice versa
                UIManager.Instance.hudWindow.SetActive(false);
            }
            else
            {
                UIManager.Instance.CloseSelectWindow();
                UIManager.Instance.CloseAllInventoryWindows();
                UIManager.Instance.hudWindow.SetActive(true);
            }
        }
    }
}
