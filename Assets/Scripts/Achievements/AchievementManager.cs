
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class AchievementManager : UdonSharpBehaviour
{
    [SerializeField] private Achievement[] achievements;

    private int _totalAchievments;
    private float _percentageComplete = 0F;
    private bool _initialized = false;

    public int TotalAchievments => _totalAchievments;
    public bool Initialized => _initialized;
    
    private void Start()
    {
        _totalAchievments = achievements.Length;
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        SendCustomEventDelayedSeconds(nameof(_initialized), 5F);
    }

    public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
    {
        // sets _initialized to false and re initalizes
    }

    public void Initialize()
    {
        foreach (var achievement in achievements)
        {
            // check achievements are ready to be fired and used
            // accumalate the percentage
            // if an achievment failes, percentage reset and resend event recursion
        }
        // If everything works, initliaze true
    }

    public float _GetCompletionPercentage()
    {
        return 0F;
    }
}
