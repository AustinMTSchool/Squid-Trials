
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KillsAchievement : Achievement
{
    [SerializeField] private int totalKills;

    public override void Start()
    {
        progress.maxValue = totalKills;
        base.Start();
    }

    public override void _Init(int kills)
    {
        progress.value = Math.Min(kills, totalKills);
        current.text = kills.ToString();
        total.text = totalKills.ToString();
        base._Init(kills);
    }

    public void _UpdateCompletion(int kills)
    {
        progress.value = Math.Min(kills, totalKills);
        current.text = progress.value.ToString();
        if (kills >= totalKills && _isCompleted == false)
        {
            if (!_isInitialized) return;
            _Complete();
        }
    }
}
