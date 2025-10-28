using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DeathStat : UdonSharpBehaviour
{
    [UdonSynced] private int _death = 0;

    public int Deaths => _death;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        RequestSerialization();
    }

    public void AddDeath(int amount)
    {
        _death += amount;
        RequestSerialization();
    }
    
}
