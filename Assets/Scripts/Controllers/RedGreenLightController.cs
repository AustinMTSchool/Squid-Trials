using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RedGreenLightController : GameController
{
    [SerializeField] private float graceTime = 0.5F;
    [SerializeField] private GameObject barrier;
    [SerializeField] private FinishZone finishZone;
    [SerializeField] private ActiveZone activeZone;
    [SerializeField] private TrafficLightManager trafficLightManager;
    
    
    [Space, Header("Timers RGL")]
    [SerializeField] private int minIntervalTime = 5;
    [SerializeField] private int maxIntervalTime = 9;

    [Space, Header("Light")] 
    [SerializeField] private Renderer lightMesh;
    [SerializeField] private Material noneLight;
    [SerializeField] private Material greenLight;
    [SerializeField] private Material redLight;

    [Space, Header("Animators")]
    [SerializeField] private Animator[] doorAnimators;

    [SerializeField] private Animator scareCrowAnimator;
    
    [Space, Header("Sounds")]
    [SerializeField] private Audio lightSwitchSound;
    
    [UdonSynced] private int _currentIntervalTime = 3;
    [UdonSynced] private Light _light;
    [UdonSynced] private bool _areLightsSwitching;
    [UdonSynced] private bool _deadlyMovement;
    [UdonSynced] private ScareCrowState _scareCrowState;
    
    private System.Random random = new System.Random();
    private int _nextLightSwitchTime;
    
    public bool DeadlyMovement => _deadlyMovement;
    public GameObject Barrier => barrier;
    public Animator[] DoorAnimator => doorAnimators;
    
    private void Start()
    {
        random = new System.Random();
    }
    
    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.isLocal) return;
        
        base.OnMasterTransferred(newMaster);
        if (game.State == GameState.Active)
        {
            if (_areLightsSwitching)
            {
                LightSwitchGameNetworked();
            }
        }
    }
    
    public override void OnDeserialization()
    {
        base.OnDeserialization();
        _UpdateLights();
        _UpdateScareCrow();
    }

    public override void _Begin()
    {
        base._Begin();
        
        _SetDoors(true);
        barrier.SetActive(false);

        if (!Networking.LocalPlayer.isMaster) return;
        
        _areLightsSwitching = true;
        _nextLightSwitchTime = _currentGameTime - 3;
        
        RequestSerialization();
        
        SendCustomEventDelayedSeconds(nameof(LightSwitchGameNetworked), 3);
    }
    
    public override void _OnTick()
    {
        base._OnTick();
    }

    public override void _End()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EndLocalSide));
        
        _currentIntervalTime = _currentGameTime;
        _areLightsSwitching = false;
        _light = Light.None;
        _scareCrowState =  ScareCrowState.None;
        
        _UpdateScareCrow();
        _UpdateLights();
        
        RequestSerialization();
        base._End();
    }

    [NetworkCallable]
    public void EndLocalSide()
    {
        barrier.SetActive(true);
        _SetDoors(false);
    }
    
    public void LightSwitchGameNetworked()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        if (!_areLightsSwitching) return;
        
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PlayLightSwitchSound));
        _currentIntervalTime = random.Next(minIntervalTime, maxIntervalTime);
        
        if (_currentIntervalTime >= _currentGameTime)
        {
            _currentIntervalTime = _currentGameTime;
            Debug.Log("_currentIntervalTime >= _currentGameTime");
        }

        if (_light == Light.None || _light == Light.Green)
        {
            _scareCrowState = ScareCrowState.Looking;
            _light = Light.Red;
            SendCustomEventDelayedSeconds(nameof(GracePeriodEnd), graceTime);
        }
        else
        {
            _scareCrowState = ScareCrowState.Hiding;
            _light = Light.Green;
            trafficLightManager.SetGreen();
            _deadlyMovement = false;
        }
        
        _UpdateScareCrow();
        _UpdateLights();
        
        RequestSerialization();
        
        if (_currentGameTime > 0)
        {
            SendCustomEventDelayedSeconds(nameof(LightSwitchGameNetworked), _currentIntervalTime);
        }
    }

    public void GracePeriodEnd()
    {
        _deadlyMovement = true;
        RequestSerialization();
    }

    public override void PlayerOutOfGame(string id)
    {
        base.PlayerOutOfGame(id);
    }

    public override void PlayerActiveGame(string id)
    {
        base.PlayerActiveGame(id);
    }

    private void _UpdateLights()
    {
        if (_light == Light.None)
        {
            lightMesh.material = noneLight;
            trafficLightManager.SetYellow();
        }
        else if (_light == Light.Green)
        {
            lightMesh.material = greenLight;
            trafficLightManager.SetGreen();
        }
        else
        {
            lightMesh.material = redLight;
            trafficLightManager.SetRed();
        }
    }

    public void _ResetLights()
    {
        lightMesh.material = noneLight;
        trafficLightManager.Reset();
    }

    private void _UpdateScareCrow()
    {
        if (_scareCrowState == ScareCrowState.None)
            scareCrowAnimator.Play("none");
        else if (_scareCrowState == ScareCrowState.Looking)
            scareCrowAnimator.Play("looking");
        else
            scareCrowAnimator.Play("hiding");
    }

    public void _SetDoors(bool state)
    {
        foreach (var door in doorAnimators)
        {
            if (state)
                door.Play("open");
            else
                door.Play("close");
        }
    }

    [NetworkCallable]
    public void PlayLightSwitchSound()
    {
        if (!application.Player.IsInGames) return;
        lightSwitchSound.Play();
    }
}

public enum Light
{
    None,
    Green = 1,
    Red = 2
}

public enum ScareCrowState
{
    None,
    Looking,
    Hiding
}