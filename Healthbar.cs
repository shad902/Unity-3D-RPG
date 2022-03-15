using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider; //health slider control call reference to slider in Unity
    public Gradient gradient; //sets gradient for health bar so it changes colors 
    public Image fill; //this is our healthbar fill image we reference

    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;

        fill.color=gradient.Evaluate(1f); //makes gradient set to 1 which is green in our case for max health
    }
    public void SetHealth(int currentHealth) //adjusts slider value as current health
    {
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue); //changes our slider between 0 and 1 for color 
    }
}
