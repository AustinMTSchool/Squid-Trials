
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class DeahZone : UdonSharpBehaviour
{
    [SerializeField] private Application application;
    [SerializeField] private Game game;
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log("Player entered");
        if (!player.isLocal || !application.Player.IsInGames) return;

        application.Player.Health._SetHealth(0);
            
        Debug.Log("Player is dead");
        application.GameManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(application.GameManager.PlayerRemoveFromGame), $"{Networking.LocalPlayer.playerId}");
        application.Player.VRCPlayerApi.SetWalkSpeed(0);
        application.Player.VRCPlayerApi.SetRunSpeed(0);
        SendCustomEventDelayedSeconds(nameof(_ManagePlayer), 3);
    }

    public void _ManagePlayer()
    {
        application.Player.VRCPlayerApi.TeleportTo(application.GameManager.SpawnPoint.position, Quaternion.identity);
        application.Player.PlayerEffects._Reset();
    }
}
