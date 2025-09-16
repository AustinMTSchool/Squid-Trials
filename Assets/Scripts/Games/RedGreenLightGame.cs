
using System;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RedGreenLightGame : Game
{
    [UdonSynced] private bool _isDoorsOpened = false;
    private RedGreenLightController _rgController;
    
    [Space, Header("Sounds")]
    [SerializeField] private Audio doorSound;

    private void Start()
    {
        if (Utilities.IsValid(gameController))
        {
            _rgController = (RedGreenLightController) gameController;
        }
    }

    public override void _BeginIntro()
    {
        base._BeginIntro();
        
        if (!Networking.LocalPlayer.isMaster) return;
        _isDoorsOpened = false;
    }

    public override void _OnIntroTick()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        base._OnIntroTick();
        
        if (_isIntro && _currentIntroTime > 0)
        {
            if (_currentIntroTime == 10)
            {
                _isDoorsOpened = true;
                _UpdateDoors();
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PlayDoorSounds));
            }
        }
    }
    
    public override void _Stop()
    {
        base._Stop();
    }

    public override void _Outro()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        base._Outro();
    }

    public override void _OnOutroTick()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        base._OnOutroTick();

        if (_currentOutroTIme <= 0)
        {
            _isDoorsOpened = false;
        }
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        base.OnMasterTransferred(newMaster);
    }

    public override void OnDeserialization()
    {
        base.OnDeserialization();
        _UpdateDoors();
    }

    private void _UpdateDoors()
    {
        Debug.Log("Updating Doors");
        _rgController._SetDoors(_isDoorsOpened);
    }

    [NetworkCallable]
    public void PlayDoorSounds()
    {
        doorSound.Play();
    }
}
