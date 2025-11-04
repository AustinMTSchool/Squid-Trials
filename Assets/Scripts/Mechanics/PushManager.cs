
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PushManager : UdonSharpBehaviour
{
    [SerializeField] private int cooldown;
    [SerializeField] private HudManager hudManager;
    
    [Header("Desktop"), Space(1)] 
    [SerializeField] private Transform desktopHandContainer;
    [SerializeField] private DesktopHand desktopHand;
    
    [Header("Virtual Reality"), Space(1)]
    [SerializeField] private Transform vrHandContainer;
    [SerializeField] private Transform vrHandContainerLeft;
    [SerializeField] private Transform vrHandContainerRight;
    [SerializeField] private HandVR leftHandVR;
    [SerializeField] private HandVR rightHandVR;
    
    private int _currentCooldown;
    private bool _isOnCooldown;
    private VRCPlayerApi _player;

    private void Start()
    {
        _player = Networking.LocalPlayer;
        
        if (!_player.IsUserInVR())
        {
            desktopHandContainer.gameObject.SetActive(true);
            _UpdateDesktopHand();
        }
        else
        {
            vrHandContainer.gameObject.SetActive(true);
            _UpdateVRHandLeft();
            _UpdateVRHandRight();
        }
    }

    public void _UpdateDesktopHand()
    {
        var position = _player.GetPosition();
        var rotation = _player.GetRotation();
        
        desktopHandContainer.SetPositionAndRotation(position, rotation);
        
        SendCustomEventDelayedSeconds(nameof(_UpdateDesktopHand), 0.1F);
    }

    public void _UpdateVRHandLeft()
    {
        VRCPlayerApi.TrackingData data = _player.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);
        vrHandContainerLeft.SetPositionAndRotation(data.position, data.rotation);
        SendCustomEventDelayedSeconds(nameof(vrHandContainerLeft), 0.1F);
    }
    
    public void _UpdateVRHandRight()
    {
        VRCPlayerApi.TrackingData data = _player.GetTrackingData(VRCPlayerApi.TrackingDataType.RightHand);
        vrHandContainerLeft.SetPositionAndRotation(data.position, data.rotation);
        SendCustomEventDelayedSeconds(nameof(vrHandContainerRight), 0.1F);
    }
    
    public override void InputUse(bool value, UdonInputEventArgs args)
    {
        if (!value) return;
        
        Debug.Log("Pushing time: " + _currentCooldown);
        Debug.Log($"Args: {args.handType}");
        
        if (_isOnCooldown) return;
        
        if (!Networking.LocalPlayer.IsUserInVR())
        {
            _isOnCooldown = true;
            desktopHand._Push();
        }
        else
        {
            Debug.Log($"Used in VR");
            _isOnCooldown = true;
            if (args.handType == HandType.LEFT)
            {
                leftHandVR._Push();
            }
            else
            {
                rightHandVR._Push();
            }
        }
        
        hudManager.Hud.SetPush(0);
        SendCustomEventDelayedSeconds(nameof(_CooldownTick), 1F);
    }

    public void _CooldownTick()
    {
        _currentCooldown++;
        hudManager.Hud.SetPush(_currentCooldown);
        
        if (_currentCooldown >= cooldown)
        {
            _isOnCooldown = false;
            _currentCooldown = 0;
        }
        else
        {
            SendCustomEventDelayedSeconds(nameof(_CooldownTick), 1F);
        }
    }
}
