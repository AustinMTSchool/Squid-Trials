
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GameController : UdonSharpBehaviour
{
    [SerializeField] protected Application application;
    [SerializeField] protected GameManager gameManager;
    [SerializeField] protected Game game;
    
    [Space, Header("Timers")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int gameTime = 120;

    [Space, Header("Sounds")] 
    [SerializeField] protected Audio clockSound;
    
    [Space, Header("Overrides")] 
    protected DataList _currentPlayersInGame = new DataList();
    
    [UdonSynced] protected string _jsonCurrentPlayersInGame;
    [UdonSynced] protected int _currentGameTime;
    [UdonSynced] protected bool _forceEnd;
    
    public DataList CurrentPlayersInGame => _currentPlayersInGame;

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.isLocal) return;

        if (game.State == GameState.Active)
        {
            _OnTick();
        }
    }
    
    public override void OnPreSerialization()
    {
        if (VRCJson.TrySerializeToJson(_currentPlayersInGame, JsonExportType.Minify, out DataToken allPlayers))
            _jsonCurrentPlayersInGame = allPlayers.String;
        else
            Debug.LogError(allPlayers.ToString());
    }

    public override void OnDeserialization()
    {
        if(VRCJson.TryDeserializeFromJson(_jsonCurrentPlayersInGame, out DataToken jsonCurrentPlayersInGame))
            _currentPlayersInGame = jsonCurrentPlayersInGame.DataList;
        else
            Debug.LogError(jsonCurrentPlayersInGame.ToString());
        
        _UpdateClock();
        Debug.Log("Player list: " + _jsonCurrentPlayersInGame);
    }

    public virtual void _Begin()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        
        _currentGameTime = gameTime;
        _UpdateClock();
        _forceEnd = false;
        
        RequestSerialization();
        
        SendCustomEventDelayedSeconds(nameof(_OnTick), 1);
    }

    public virtual void _OnTick()
    {
        if (!Networking.LocalPlayer.isMaster) return;

        if (_forceEnd)  return;
        
        _currentGameTime--;
        _UpdateClock();
        RequestSerialization();

        if (_currentGameTime <= 0)
        {
            Debug.Log("[Controller] Game ended due to time");
            _End();
        }
        else
        {
            SendCustomEventDelayedSeconds(nameof(_OnTick), 1);
        }
    }

    public virtual void _End()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        Debug.Log("ENDING GAME _End");

        _forceEnd = true;
        _currentGameTime = gameTime;
        _UpdateClock();
        RequestSerialization();
        
        
        if (VRCJson.TrySerializeToJson(_currentPlayersInGame, JsonExportType.Minify, out DataToken allPlayers))
            _jsonCurrentPlayersInGame = allPlayers.String;
        else
            Debug.LogError(allPlayers.ToString());
        
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(KillPlayersDidNotFinish), _jsonCurrentPlayersInGame);
        
        // SendCustomEventDelayedSeconds(nameof(_Delay), 3);
        // or send the json string directly from master
        game._Outro();
    }

    public void _Delay(NetworkEventTarget target, string name)
    {
        SendCustomNetworkEvent(target, name);
    }
    
    [NetworkCallable(40)]
    public void KillPlayersDidNotFinish(string jsonPlayers)
    {
        string id = $"{Networking.LocalPlayer.playerId}";
        Debug.Log($"[GameController] You are {id}, people who didnt finish: {jsonPlayers}");
        
        var playersLeft = new DataList();
        if(VRCJson.TryDeserializeFromJson(jsonPlayers, out DataToken jsonCurrentPlayersInGame))
            playersLeft = jsonCurrentPlayersInGame.DataList;
        else
            Debug.LogError(jsonCurrentPlayersInGame.ToString());

        if (playersLeft.Contains(id))
        {
            Debug.Log("[GameController] You will die");
            application.Player.Death();
            application.GameManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(application.GameManager.PlayerRemoveFromGame), $"{Networking.LocalPlayer.playerId}");
            application.Player.VRCPlayerApi.TeleportTo(application.GameManager.SpawnPoint.position, Quaternion.identity);
            application.Player.PlayerEffects._Reset();
        }
        else
        {
            Debug.Log($"[GameController] You will not die");
        }
    }

    // when a player finishes the game
    public virtual void PlayerOutOfGame(string id)
    {
        if (_forceEnd)
        {
            Debug.Log("[GameController] Ignoring removal of player game is ending");
            return;
        }
        
        _currentPlayersInGame.Remove(id);
        RequestSerialization();

        for (int i = 0; i < _currentPlayersInGame.Count; i++)
        {
            Debug.Log($"Players: {_currentPlayersInGame[i]}");
        }

        if (_currentPlayersInGame.Count == 0)
        {
            Debug.Log("PlayerOutOfGame => _End");
            _End();
        }
    }

    // if a player crosses the finish zone but goes back into the active zone, re add them
    // to the game
    public virtual void PlayerActiveGame(string id)
    {
        if (_currentPlayersInGame.Contains(id)) return;
        
        _currentPlayersInGame.Add(id);
        RequestSerialization();
    }

    // clone all players currently going into the game
    public void AssignCurrentPlayersInGame()
    {
        _currentPlayersInGame = gameManager.AllPlayersInGame.DeepClone();
    }
    
    private void _UpdateClock()
    {
        timerText.text = Clock.ConvertInteger(_currentGameTime);
        clockSound.Play();
    }

    
}
