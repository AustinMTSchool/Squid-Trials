
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class FinishZone : UdonSharpBehaviour
{
    [SerializeField] private Game game;

    public void Begin()
    {
        
    }
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (game.State == GameState.Active)
        {
            Debug.Log("[FinishZone] PLAYER ENTERED");
            game.GameController.PlayerOutOfGame($"{player.playerId}");
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (game.State == GameState.Active)
        {
            Debug.Log("PLAYER EXITED");
            game.GameController.PlayerActiveGame($"{player.playerId}");
        }
    }
}
