
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
    [SerializeField] protected TextMeshProUGUI title;
    [SerializeField] protected TextMeshProUGUI current;
    [SerializeField] protected TextMeshProUGUI total;
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
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        if (!PlayerData.HasKey(player, achievementKey))
        {
            PlayerData.SetBool(achievementKey, false);
        }
        
        _isCompleted = PlayerData.GetBool(player, achievementKey);
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
