using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Singleton<HealthBar>
{
   public Slider slider;
   public void SetMaxHealth(int max)
   {
       slider.maxValue = max;
       slider.value = max;
   }

   public void SetCurrentHealth(int currentHealth)
   {
       slider.value = currentHealth;
   }
}
