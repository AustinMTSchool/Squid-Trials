
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Hud : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider pushSlider;
    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI nextLevel;
    [SerializeField] private TextMeshProUGUI expNeeded;

    public void SetMaxHealth(int value)
    {
        healthText.text = value.ToString();
        healthSlider.maxValue = value;
        healthSlider.value = value;
    }
    
    public void SetHealth(int health)
    {
        healthText.text = health.ToString();
        healthSlider.value = health;
    }

    public void SetPush(int value)
    {
        pushSlider.value = value;
    }

    public void _SetExperience(int value)
    {
        experienceSlider.value = value;
    }

    public void _SetExperienceMax(int value)
    {
        experienceSlider.maxValue = value;
    }

    public void _SetCurrentLevel(int value)
    {
        currentLevel.text = value.ToString();
    }
    
    public void _SetNextLevel(int value)
    {
        nextLevel.text = value.ToString();
    }

    public void _SetExpNeeded(int value)
    {
        expNeeded.text = value.ToString();
    }
}
