
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BandageUsable : Usable
{
    private float _walkSpeed;
    private float _runSpeed;
    
    public override void Use()
    {
        base.Use();
        application.UsableManager.SetUsing(true);
        _walkSpeed = application.Player.VRCPlayerApi.GetWalkSpeed();
        _runSpeed = application.Player.VRCPlayerApi.GetRunSpeed();
        application.Player.VRCPlayerApi.SetWalkSpeed(_walkSpeed / 2);
        application.Player.VRCPlayerApi.SetRunSpeed(_runSpeed / 2);
        SendCustomEventDelayedSeconds(nameof(RestoreHealth), 3F);
    }

    public void RestoreHealth()
    {
        application.Player.VRCPlayerApi.SetWalkSpeed(_walkSpeed);
        application.Player.VRCPlayerApi.SetRunSpeed(_runSpeed);
        application.UsableManager.SetUsing(false);
    }
}
