
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerEffects : UdonSharpBehaviour
{
    private VRCPlayerApi _player;
    private readonly float walkSpeed = 2;
    private readonly float runSpeed = 4;

    private void Start()
    {
        _player = Networking.LocalPlayer;
    }

    public void _Reset()
    {
        // TODO need to add class speeds
        _player.SetWalkSpeed(walkSpeed); // + class
        _player.SetRunSpeed(runSpeed);
    }

    public void _AddSpeed(float percent)
    {
        float currentWalk = walkSpeed;
        float currentRun = runSpeed;
        
        float addWalk = currentWalk / percent;
        float addRun = currentRun / percent;
        
        currentWalk = _player.GetWalkSpeed() + addWalk;
        currentRun = _player.GetRunSpeed() + addRun;
        
        _player.SetWalkSpeed(currentWalk);
        _player.SetRunSpeed(currentRun);
    }

    public void _RemoveSpeed(float percent)
    {
        float currentWalk = walkSpeed;
        float currentRun = runSpeed;
        
        float addWalk = currentWalk / percent;
        float addRun = currentRun / percent;
        
        currentWalk = _player.GetWalkSpeed() - addWalk;
        currentRun = _player.GetRunSpeed() - addRun;
        
        Debug.Log($"Speed: {currentRun}");
        
        _player.SetWalkSpeed(currentWalk);
        _player.SetRunSpeed(currentRun);
    }
}
