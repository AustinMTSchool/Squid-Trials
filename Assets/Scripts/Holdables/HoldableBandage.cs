
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HoldableBandage : Holdable
{
    [SerializeField] protected Audio bandagingSound;
    [SerializeField] private float usageTime;
    [SerializeField] private int healthAmount;

    private float _playerWalkSpeed;
    private float _playerRunSpeed;

    public override void OnPickupUseUp()
    {
        if (application.Player.IsUsingItem) return;
        
        application.Player.SetUsingItem(true);
        bandagingSound.Play();

        VRCPlayerApi player = Networking.LocalPlayer;
        _playerWalkSpeed = player.GetWalkSpeed();
        _playerRunSpeed = player.GetRunSpeed();
        player.SetWalkSpeed(_playerWalkSpeed / 2);
        player.SetRunSpeed(_playerRunSpeed / 2);
        
        SendCustomEventDelayedSeconds(nameof(FinishedUsingItem), usageTime);
    }

    public void FinishedUsingItem()
    {
        Debug.Log("USING BANDAGE ITEM");
        VRCPlayerApi player = Networking.LocalPlayer;
        player.SetWalkSpeed(_playerWalkSpeed);
        player.SetRunSpeed(_playerRunSpeed);
        this.player.Health._AddHealth(healthAmount);
        base.OnPickupUseUp();
    }
}
