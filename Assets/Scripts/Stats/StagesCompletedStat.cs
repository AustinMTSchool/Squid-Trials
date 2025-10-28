
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StagesCompletedStat : UdonSharpBehaviour
{
    [UdonSynced] private int _stagesCompleted = 0;

    public int StagesCompleted => _stagesCompleted;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        RequestSerialization();
    }

    public void AddStagesCompleted(int amount)
    {
        _stagesCompleted += amount;
        RequestSerialization();
    }
}
