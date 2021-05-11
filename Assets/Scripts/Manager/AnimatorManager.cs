using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator anim;
    #region Animator States
    public static string Locomotion_STATE = "Locomotion";
    public static string Falling_STATE = "Falling";
    public static string Land_STATE = "Land";
    public static string Empty_STATE = "Empty";
    public static string Rolling_STATE = "Rolling";
    public static string Backstep_STATE = "Backstep";
    public static string Damage_STATE = "Damage_01";
    public static string Death_STATE = "Death";
    public static string Pick_Up_Item_STATE = "Pick Up Item";
    public static string Jump_STATE = "Jump";
    #endregion
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

}
