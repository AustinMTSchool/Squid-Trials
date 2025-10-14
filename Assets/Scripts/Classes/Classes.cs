
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Classes : UdonSharpBehaviour
{
    [SerializeField] private ClassesManager classesManager;
    
    [SerializeField] private string className;
    [SerializeField] private string classDescription;
    [SerializeField] private Sprite classIcon;
    [SerializeField] private float agilityAmount;
    [SerializeField] private int healthAmount;
    [SerializeField] private int defensePercentage;
    [SerializeField] private float antiKnockbackPercentage;
    [SerializeField] private float forceKnockbackPercentage;
    
    [Header("UI"), Space(1)]
    [SerializeField] private TextMeshProUGUI classNameText;
    [SerializeField] private TextMeshProUGUI classDescriptionText;
    [SerializeField] private Image classImageIcon;
    [SerializeField] private TextMeshProUGUI agilityAmountText;
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private TextMeshProUGUI defensePercentageText;
    [SerializeField] private TextMeshProUGUI antiKnockbackPercentageText;
    [SerializeField] private TextMeshProUGUI forceKnockbackPercentageText;

    public string ClassName => className;
    public float AgilityAmount => agilityAmount;
    public float HealthAmount => healthAmount;
    public float DefensePercentage => defensePercentage;
    public float AntiKnockbackPercentage => antiKnockbackPercentage;
    public float ForceKnockbackPercentage => forceKnockbackPercentage;

    private void Start()
    {
        classNameText.text = className;
        classDescriptionText.text = classDescription;
        classImageIcon.sprite = classIcon;
        agilityAmountText.text += $" " + agilityAmount;
        healthAmountText.text += $" " + healthAmount;
        defensePercentageText.text += $" {defensePercentage}%";
        antiKnockbackPercentageText.text += $" {antiKnockbackPercentage}%";
        forceKnockbackPercentageText.text += $" {forceKnockbackPercentage}%";
    }

    public void ApplyEffects(Player player)
    {
        player.Health.AddMaxHealth(healthAmount);
        player.PlayerEffects._AddSpeed(agilityAmount);
        player.Health._AddDefense(defensePercentage);
        Debug.Log($"{className} was equipped!");
    }

    public void RemoveEffects(Player player)
    {
        player.Health.RemoveMaxHealth(healthAmount);
        player.PlayerEffects._RemoveSpeed(agilityAmount);
        player.Health._RemoveDefense(defensePercentage);
        Debug.Log($"{className} was un-equipped!");
    }

    public void OnClick()
    {
        classesManager.OnClassSelection(this);
    }
}
