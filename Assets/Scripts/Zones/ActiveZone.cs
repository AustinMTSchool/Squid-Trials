
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class ActiveZone : UdonSharpBehaviour
{
    [SerializeField] private Application application;
    [SerializeField] private Game game;
    [SerializeField] private RedGreenLightController controller;
    [SerializeField] private float updateInterval = 0.05f;
    
    private VRCPlayerApi _player;
    private bool _isInZone = false;
    
    private void Start()
    {
        _player = Networking.LocalPlayer;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        _isInZone = true;
        if (game.State != GameState.ACTIVE) return;

        if (_player.IsUserInVR())
            WatchPlayerMovementVR();
        else
            WatchPlayerMovement();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        _isInZone = false;
    }

    public void WatchPlayerMovement()
    {
        if (!_isInZone) return;
        if (game.State != GameState.ACTIVE) return;
        
        if (controller.DeadlyMovement)
        {
            if (_player.GetVelocity().magnitude != 0F)
            {
                if (application.Player.Health._RemoveHealth(20))
                {
                    application.GameManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(application.GameManager.PlayerRemoveFromGame), $"{Networking.LocalPlayer.playerId}");
                    application.Player.VRCPlayerApi.TeleportTo(application.GameManager.SpawnPoint.position, Quaternion.identity);
                    application.Player.PlayerEffects._Reset();
                }
                else
                {
                    application.Player.PlayerEffects._RemoveSpeed(8F);
                }
            }
        }
        SendCustomEventDelayedSeconds(nameof(WatchPlayerMovement), updateInterval);
    }
    
    public void WatchPlayerMovementVR()
    {
        if (!_isInZone) return;
        
        SendCustomEventDelayedSeconds(nameof(WatchPlayerMovementVR), updateInterval);
    }
}
