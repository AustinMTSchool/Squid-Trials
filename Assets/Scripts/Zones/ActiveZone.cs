
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
    [SerializeField] private ShootOutManager shootOutManager;
    [SerializeField] private bool canFire = true;
	[SerializeField] int shootingInterval = 1;
    
    private VRCPlayerApi _player;
    private bool _isInZone = false;
    private bool _hasShotInInterval = false;
    
    public bool HashShotInInterval => _hasShotInInterval;
    
    private void Start()
    {
        _player = Networking.LocalPlayer;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        _isInZone = true;
        if (game.State != GameState.Active) return;

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
        if (game.State != GameState.Active) return;
        
        if (controller.DeadlyMovement)
        {
            if (_player.GetVelocity().magnitude != 0F)
            {
				if (!_hasShotInInterval)
				{
					_hasShotInInterval = true;
					shootOutManager.Fire();
				}
                
                if (application.Player.Health._RemoveHealth(20))
                {
                    application.GameManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(application.GameManager.PlayerRemoveFromGame), $"{Networking.LocalPlayer.playerId}");
                    application.Player.VRCPlayerApi.SetWalkSpeed(0);
                    application.Player.VRCPlayerApi.SetRunSpeed(0);
                    SendCustomEventDelayedSeconds(nameof(_ManagePlayer), 3);
                }
                else
                {
                    if (application.Player.IsPushed)
                    {
                        Debug.Log("Pushed 1.5%");
                        application.Player.PlayerEffects._RemoveSpeedPercent(1.5F);
                    }
                    else
                    {
                        Debug.Log("Red 8%");
                        application.Player.PlayerEffects._RemoveSpeedPercent(8F);
                    }
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

    public void _ManagePlayer()
    {
        application.Player.VRCPlayerApi.TeleportTo(application.GameManager.SpawnPoint.position, Quaternion.identity);
        application.Player.PlayerEffects._Reset();
    }

    public void _SetHashShotInInterval(bool value)
    {
        _hasShotInInterval = value;
    }
}
