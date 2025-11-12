
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HoldableSpeedPotion : Holdable
{
    [SerializeField] protected Audio potionSound;
    [SerializeField] private float usageTime = 2;
    [SerializeField] private int speedTime = 30;
    [SerializeField] private float speedBoostAmount = 0.2F;

    private float _playerWalkSpeed;
    private float _playerRunSpeed;
    
    public override void OnPickupUseUp()
    {
        if (application.Player.IsUsingItem) return;
        
        application.Player.SetUsingItem(true);
        potionSound.Play();

        VRCPlayerApi player = Networking.LocalPlayer;
        _playerWalkSpeed = player.GetWalkSpeed();
        _playerRunSpeed = player.GetRunSpeed();
        player.SetWalkSpeed(_playerWalkSpeed / 2);
        player.SetRunSpeed(_playerRunSpeed / 2);
        
        SendCustomEventDelayedSeconds(nameof(FinishedUsingItem), usageTime);
    }

    public void FinishedUsingItem()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        player.SetWalkSpeed(_playerWalkSpeed);
        player.SetRunSpeed(_playerRunSpeed);
        this.player.PlayerEffects._AddSpeedDuration(speedTime, speedBoostAmount);
        base.OnPickupUseUp();
    }
}
