
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class Achievement : UdonSharpBehaviour
{
    [SerializeField] protected string achievementName;
    [SerializeField] protected string achievementDescription;
    [SerializeField] protected Color nonCompleteColor = new Color(0.541F,0.541F,0.541F);
    [SerializeField] protected Color completeColor = new Color(0.149F,0.596F,0.596F);
    [SerializeField] protected TextMeshProUGUI title;
    [SerializeField] protected TextMeshProUGUI current;
    [SerializeField] protected TextMeshProUGUI total;
    [SerializeField] protected TextMeshProUGUI description;
    [SerializeField] protected Image icon;
    [SerializeField] protected Image outline;
    [SerializeField] protected Slider progress;
    [SerializeField] protected string achievementKey;
    [SerializeField] protected Reward reward;

    protected bool _isCompleted = false;
    protected bool _isInitialized = false;
    
    public bool IsCompleted => _isCompleted;
    public bool IsInitialized => _isInitialized;

    public virtual void Start()
    {
        title.text = achievementName;
        description.text  = achievementDescription;
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        if (!PlayerData.HasKey(player, achievementKey))
        {
            PlayerData.SetBool(achievementKey, false);
        }
        
        _isCompleted = PlayerData.GetBool(player, achievementKey);

        if (_isCompleted)
        {
            outline.color = completeColor;
            icon.color = completeColor;
        }
        
        _isInitialized = true;
    }

    protected virtual void _Complete()
    {
        PlayerData.SetBool(achievementKey, true);
        _isCompleted = true;
        reward._GiveReward();
    }

    public virtual void _Init()
    {
        reward._Init();
    }
    
    public virtual void _Init(int amount)
    {
        reward._Init();
    }
}
