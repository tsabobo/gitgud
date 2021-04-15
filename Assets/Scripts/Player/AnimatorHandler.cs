using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    PlayerManager playerManager;
    public Animator anim;
    InputHandler inputHandler;
    PlayerLocomotion playerLocomotion;
    int vertical;
    int horizontal;
    public static string Falling_STATE = "Falling";
    public static string Land_STATE = "Land";
    public static string Locomotion_STATE = "Locomotion";
    public static string Empty_STATE = "Empty";
    public static string Rolling_STATE = "Rolling";
    public static string Backstep_STATE = "Backstep";
    public static string Damage_STATE = "Damage_01";
    public static string Death_STATE = "Death";
    public static string Pick_Up_Item_STATE = "Pick Up Item";
    public bool canRotate;
    public void Initialize()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();

        anim = GetComponent<Animator>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnim, 0.2f);

        #region MY FIX
        // Fix bug that keeps falling from small platform, by adding transition betwenn "Falling" and "Empty" state
        if (targetAnim == Locomotion_STATE)
        {
            anim.SetBool("isGrounded", true);
        }
        if (targetAnim == Falling_STATE)
        {
            anim.SetBool("isGrounded", false);
        }
        #endregion
    }

    public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
    {
        #region Vertical
        float v = 0;
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            v = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            v = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }
        #endregion

        #region Horizontal
        float h = 0;
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            h = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }
        #endregion

        if (isSprinting)
        {
            v = 2;
            h = horizontalMovement;
        }
        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotate()
    {
        canRotate = false;
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public bool CheckCurrentAnimationState(string name)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.isInteracting == false)
            return;

        float delta = Time.deltaTime;
        playerLocomotion.rigidbody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.rigidbody.velocity = velocity;

    }
}
