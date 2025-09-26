
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PushManager : UdonSharpBehaviour
{
    [SerializeField] private int cooldown;

    [Header("Desktop"), Space(1)] 
    [SerializeField] private Transform desktopHandContainer;
    [SerializeField] private DesktopHand desktopHand;
    
    private int _currentCooldown;
    private VRCPlayerApi _player;

    private void Start()
    {
        _player = Networking.LocalPlayer;
        
        if (!_player.IsUserInVR())
        {
            _UpdateDesktopHand();
        }
    }

    public void _UpdateDesktopHand()
    {
        var position = _player.GetPosition();
        var rotation = _player.GetRotation();
        
        desktopHandContainer.SetPositionAndRotation(position, rotation);
        
        SendCustomEventDelayedSeconds(nameof(_UpdateDesktopHand), 0.1F);
    }
    
    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        if (!value) return;
        
        Debug.Log("Pushing time: " + _currentCooldown);
        if (_IsOnCooldown()) return;
        
        if (!Networking.LocalPlayer.IsUserInVR())
        {
            desktopHand._Push();
        }
        else
        {
            
        }
        
        SendCustomEventDelayedSeconds(nameof(_CooldownTick), 1F);
    }

    public void _CooldownTick()
    {
        _currentCooldown++;
        if (_currentCooldown >= cooldown)
        {
            _currentCooldown = 0;
        }
        else
        {
            SendCustomEventDelayedSeconds(nameof(_CooldownTick), 1F);
        }
    }

    private bool _IsOnCooldown()
    {
        return _currentCooldown != 0;
    }
}
