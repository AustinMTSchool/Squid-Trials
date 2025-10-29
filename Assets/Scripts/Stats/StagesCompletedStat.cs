
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StagesCompletedStat : UdonSharpBehaviour
{
    [SerializeField] private PointStat pointStat;
    
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
        pointStat.AddPoints(amount * 150);
        Debug.Log("STAGE WAS COMPLETED");
        RequestSerialization();
    }
}
