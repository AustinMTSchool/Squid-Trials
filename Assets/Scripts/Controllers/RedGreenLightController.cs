
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RedGreenLightController : GameController
{
    [SerializeField] private Game game;
    [SerializeField] private float graceTime = 0.5F;
    [SerializeField] private GameObject barrier;
    [SerializeField] private FinishZone finishZone;
    [SerializeField] private ActiveZone activeZone;
    
    [Space, Header("Timers")]
    [SerializeField] private int minTime = 3;
    [SerializeField] private int maxTime = 6;
    [SerializeField] private int gameTime = 120;

    [Space, Header("Light")] 
    [SerializeField] private Renderer lightMesh;
    [SerializeField] private Material noneLight;
    [SerializeField] private Material greenLight;
    [SerializeField] private Material redLight;

    [UdonSynced] private int _currentIntervalTime = 3;
    [UdonSynced] private Light _light;
    [UdonSynced] private bool _areLightsSwitching;
    [UdonSynced] private int _currentGameTime;
    [UdonSynced] private bool _deadlyMovement;
    [UdonSynced] private bool _forceEnd;
    
    private System.Random random = new System.Random();
    
    public bool DeadlyMovement => _deadlyMovement;
    public GameObject Barrier => barrier;
    private void Start()
    {
        random = new System.Random();
    }

    public override void Begin()
    {
        base.Begin();
        barrier.SetActive(false);

        if (!Networking.LocalPlayer.isMaster) return;
            
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        
        _areLightsSwitching = true;
        _currentGameTime = gameTime;
        RequestSerialization();
        SendCustomEventDelayedSeconds(nameof(LightSwitchGameNetworked), 3);
    }

    public override void End()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EndNetwork));
        _currentIntervalTime = _currentGameTime;
        _areLightsSwitching = false;
        _light = Light.NONE;
        UpdateLight();
        RequestSerialization();
        
        game.EndGame();
    }

    [NetworkCallable]
    public void EndNetwork()
    {
        barrier.SetActive(true);
    }

    public void LightSwitchGameNetworked()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (_currentGameTime <= 0)
        {
            Debug.Log("Game ended due to time");
            End();
            return;
        }
        
        _currentGameTime -= _currentIntervalTime;
        _currentIntervalTime = random.Next(minTime, maxTime);
        
        if (_currentIntervalTime >= _currentGameTime)
        {
            _currentIntervalTime = _currentGameTime;
            Debug.Log("_currentIntervalTime >= _currentGameTime");
        }

        if (_light == Light.NONE || _light == Light.GREEN)
        {
            _light = Light.RED;
            SendCustomEventDelayedSeconds(nameof(GracePeriodEnd), graceTime);
        }
        else
        {
            _light = Light.GREEN;
            _deadlyMovement = false;
        }
        
        UpdateLight();
        Debug.Log("Time: " + _currentGameTime);
        Debug.Log("Interval: " + _currentIntervalTime);
        
        RequestSerialization();
        SendCustomEventDelayedSeconds(nameof(LightSwitchGameNetworked), _currentIntervalTime);
    }

    public void GracePeriodEnd()
    {
        _deadlyMovement = true;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        UpdateLight();
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.isLocal) return;
        
        if (game.State == GameState.ACTIVE)
        {
            if (_areLightsSwitching)
            {
                LightSwitchGameNetworked();
            }
        }
    }

    public override void PlayerOutOfGame(string id)
    {
        base.PlayerOutOfGame(id);
        if (_currentPlayersInGame.Count == 0) End();
    }

    public override void PlayerActiveGame(string id)
    {
        base.PlayerActiveGame(id);
    }

    private void UpdateLight()
    {
        if (_light == Light.NONE)
            lightMesh.material =  noneLight;
        else if (_light == Light.GREEN)
            lightMesh.material = greenLight;
        else
            lightMesh.material = redLight;
    }
    
}

public enum Light
{
    NONE,
    GREEN = 1,
    RED = 2
}