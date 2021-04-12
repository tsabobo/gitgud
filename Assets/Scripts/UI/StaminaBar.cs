using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Singleton<StaminaBar>
{
   public Slider slider;
   public void SetMaxStamina(int max)
   {
       slider.maxValue = max;
       slider.value = max;
   }

   public void SetCurrentStamina(int currentStamina)
   {
       slider.value = currentStamina;
   }
}
