using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHandler : Singleton<InputHandler>
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
    public bool y_Input;
    public bool x_Input;
    // RB button (R1)
    public bool rb_Input;
    // RT button (R2)
    public bool rt_Input;
    public bool jump_Input;
    public bool inventory_Input;
    public bool lockOn_Input;
    public bool rightStick_Right_Input;
    public bool rightStick_Left_Input;
    public bool d_Pad_Up;
    public bool d_Pad_Down;
    public bool d_Pad_Left;
    public bool d_Pad_Right;
    #endregion
    public bool twoHandFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public bool inventoryFlag;
    public bool rollFlag;
    public bool lockOnFlag;
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
            inputActions.PlayerMovement.LockOnTargetLeft.performed += i => rightStick_Left_Input = true;
            inputActions.PlayerMovement.LockOnTargetRight.performed += i => rightStick_Right_Input = true;

            inputActions.PlayerAction.RB.performed += i => rb_Input = true;
            inputActions.PlayerAction.RT.performed += i => rt_Input = true;

            inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
            inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;

            inputActions.PlayerAction.A.performed += i => a_Input = true;
            inputActions.PlayerAction.Inventory.performed += i => inventory_Input = true;
            inputActions.PlayerAction.Jump.performed += i => jump_Input = true;
            inputActions.PlayerAction.LockOn.performed += i => lockOn_Input = true;
            inputActions.PlayerAction.Y.performed += i => y_Input = true;
            inputActions.PlayerAction.X.performed += i => x_Input = true;
        }

        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        HandleMoveInput(delta);
        HandleRollInput(delta);
        HandleAttackInput(delta);
        HandleQuickSlotsInput();
        HandleInventoryInput();
        HandleLockOnInput();
        HandleTowHandInput();
    }

    private void HandleMoveInput(float delta)
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
        sprintFlag = b_Input;
        if (b_Input)
        {
            rollInputTimer += delta;
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
        if (d_Pad_Right)
        {
            playerInventory.ChangeRightHandWeapon();
        }
        else if (d_Pad_Left)
        {
            playerInventory.ChangeLeftHandWeapon();
        }
    }

    private void HandleInventoryInput()
    {
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

    public void HandleLockOnInput()
    {
        if (lockOn_Input && lockOnFlag == false)
        { 
            lockOn_Input = false;
            CameraHandler.Instance.HanldeLockOn();
            if (CameraHandler.Instance.nearestLockOnTarget != null)
            {
                CameraHandler.Instance.currentLockOnTarget = CameraHandler.Instance.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if (lockOn_Input && lockOnFlag)
        {
            lockOn_Input = false;
            lockOnFlag = false;

            CameraHandler.Instance.ClearLockOnTargets();
        }

        // If right stick pushed to left and we are currently locking
        if(lockOnFlag && rightStick_Left_Input)
        {
            rightStick_Left_Input = false;
            CameraHandler.Instance.HanldeLockOn();
            // Set lock on target on the left if exists
            if(CameraHandler.Instance.leftLockOnTarget != null)
            {
                CameraHandler.Instance.currentLockOnTarget = CameraHandler.Instance.leftLockOnTarget;
            }
        }
        
        if(lockOnFlag && rightStick_Right_Input)
        {
            rightStick_Right_Input = false;
            CameraHandler.Instance.HanldeLockOn();
            if(CameraHandler.Instance.rightLockOnTarget != null)
            {
                CameraHandler.Instance.currentLockOnTarget = CameraHandler.Instance.rightLockOnTarget;
            }
        }

        CameraHandler.Instance.SetCameraHeight();

    }

    private void HandleTowHandInput()
    {
        if(y_Input)
        {
            y_Input = false;
            twoHandFlag = !twoHandFlag;

            if(twoHandFlag)
            {
                // Enable tow handing, load only right hand
                WeaponSlotManager.Instance.LoadWeaponOnSlot(playerInventory.rightWeapon, false);

            }
            else
            {
                // load both hands
                WeaponSlotManager.Instance.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                WeaponSlotManager.Instance.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
            }
        }
    }

}
