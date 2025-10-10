
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class FinishZone : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Game game;
    [SerializeField] private GameManager gameManager;

    public void Begin()
    {
        
    }
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (game.State == GameState.Active)
        {
            Debug.Log("[FinishZone] PLAYER ENTERED");
            
            /**
            if (gameManager.NumberOfWinners == game.GameController.TotalPlayersThatCanPass)
            {
                Debug.Log("The winner is: " + player.playerId); // works so now make this player reset and show them on board win
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(AssignWinner), player.playerId);
            }
            **/
            
            game.GameController._AddCurrentTotalPlayersPassed(1);
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

    /**
    [NetworkCallable]
    public void AssignWinner(int idWinner)
    {
        var loalID = Networking.LocalPlayer.playerId;
        if (idWinner != loalID) return;
        
        // TODO do winning stuff
        Debug.Log("[FinishZone] WINNER WINNER WINNER WINNER WINNER");
        player.SetInGames(false);
        gameManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(gameManager.PlayerRemoveFromGame), $"{idWinner}");
        player.VRCPlayerApi.TeleportTo(gameManager.SpawnPoint.position, Quaternion.identity);
        player.PlayerEffects._Reset();
    }
    **/
}
