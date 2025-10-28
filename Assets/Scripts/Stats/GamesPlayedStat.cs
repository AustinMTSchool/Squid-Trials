
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GamesPlayedStat : UdonSharpBehaviour
{
    [UdonSynced] private int _gamesPlayed = 0;

    public int GamesPlayed => _gamesPlayed;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        RequestSerialization();
    }

    public void AddGamesPlayed(int amount)
    {
        _gamesPlayed += amount;
        RequestSerialization();
    }
}
