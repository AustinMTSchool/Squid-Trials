
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Game : UdonSharpBehaviour
{
    [Space, Header("Managers")]
    [SerializeField] protected Application application;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected GameController gameController;
    
    [Space, Header("Credits")]
    [SerializeField] protected string gameName = "default";
    [SerializeField] protected string description;
    
    [Space, Header("Times")]
    [SerializeField] protected int introTime = 30;
    [SerializeField] protected int outroTime = 10;
    [SerializeField] protected TextMeshProUGUI introDisplay;
    
    protected readonly string DefaultGame = "main";
    
    [UdonSynced] protected GameState state = 0;
    [UdonSynced] protected bool _forceEnd = false;
    [UdonSynced] protected int _currentIntroTime;
    [UdonSynced] protected bool _isIntro = false;
    [UdonSynced] protected int _currentOutroTIme;
    [UdonSynced] protected bool _isOutro = false;
    
    protected DataDictionary _introDic = new DataDictionary()
    {
        {15, "You will have a set amount of time to get to the end of the game. You can move on green, but must be still on red"},
        {7, "If you do not finish fast enough or break the rules you will be eliminated"}
    };

    public GameState State => state;
    public string GameName => gameName;
    public GameController GameController => gameController;
    public Transform SpawnPoint => spawnPoint;

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.isLocal) return;
        
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        
        if (_isIntro) _OnIntroTick();
        if (_isOutro) _OnOutroTick();
    }

    public override void OnDeserialization()
    {
        if (_isIntro) _UpdateIntroDisplayText();
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning player " + application.Player.VRCPlayerApi.displayName);
        application.Player.VRCPlayerApi.TeleportTo(spawnPoint.position, spawnPoint.rotation);
    }
    
    public virtual void _BeginIntro()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        Debug.Log($"[Game] _BeginIntro");
        
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        _forceEnd = false;
        _currentIntroTime = introTime;
        _isIntro = true;
        _currentOutroTIme = outroTime;
        _isOutro = false;
        
        gameController.AssignCurrentPlayersInGame();
        state = GameState.Intro;
        
        RequestSerialization();
        _OnIntroTick();
    }

    public virtual void _OnIntroTick()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        Debug.Log($"[Game] _OnIntroTick");
        
        if (_forceEnd)
        {
            Debug.Log($"[Game] Force ending");
            _currentIntroTime = 0;
            _isIntro = false;
            state = GameState.None;
            RequestSerialization();
            return;
        }

        if (_isIntro && _currentIntroTime > 0)
        {
            _currentIntroTime--;
            _UpdateIntroDisplayText();
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(_OnIntroTick), 1F);
        }
        else
        {
            _isIntro = false;
            state = GameState.Active;
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(BeginGame));
        }
    }
    
    [NetworkCallable]
    public void BeginGame()
    {
        gameController._Begin();
    }

    public virtual void _Stop()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        Debug.Log($"[Game] _Stop");
        _forceEnd = true;
        if (state == GameState.Active)
        {
            gameController._End();
        }
    }

    public virtual void _Outro()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        Debug.Log($"[Game] Outro");
        state = GameState.Outro;
        _isOutro = true;
        _currentOutroTIme = outroTime;
        RequestSerialization();
        
        _OnOutroTick();
    }

    public virtual void _OnOutroTick()
    {
        if (!Networking.LocalPlayer.isMaster) return;

        Debug.Log($"[Game] _OnOutroTick");
        if (_forceEnd)
        {
            state = GameState.None;
            _isOutro = false;
            _currentOutroTIme = 0;
            RequestSerialization();
            return;
        }

        _currentOutroTIme--;
        if (_currentOutroTIme <= 0)
        {
            Debug.Log($"[Game] Outro ended.");
            state = GameState.None;
            _isOutro = false;
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(SpawnPlayerInLobby));
            application.GameManager.StartLobby();
        }
        else
        {
            SendCustomEventDelayedSeconds(nameof(_OnOutroTick), 1F);
        }
    }

    [NetworkCallable]
    public void SpawnPlayerInLobby()
    {
        Debug.Log($"[Game] Spawning player into lobby");
        if (application.Player.IsInGames)
        {
            application.GameManager.SpawnPlayerInLobby();
        }
    }
    
    private void _UpdateIntroDisplayText()
    {
        if (_introDic.TryGetValue(_currentIntroTime, out var value))
        {
            var intro = value.String;
            introDisplay.text = intro;
        }
    }
}

public enum GameState
{
    None = 0,
    Intro = 1,
    Active = 2,
    Outro = 3
}
