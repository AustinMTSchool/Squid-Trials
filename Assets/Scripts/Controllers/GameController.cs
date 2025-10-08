
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

        _forceEnd = true;
        _currentGameTime = gameTime;
        _UpdateClock();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(KillPlayersDidNotFinish), _jsonCurrentPlayersInGame);
        
        RequestSerialization();
        game._Outro();
    }
    
    [NetworkCallable]
    public void KillPlayersDidNotFinish(string playersInGame)
    {
        var players = new DataList();
        if (VRCJson.TryDeserializeFromJson(playersInGame, out DataToken playersList))
        {
            players = playersList.DataList;
        }
        else
            Debug.LogError(playersList.ToString());

        var id = Networking.LocalPlayer.playerId;

        if (players.Contains(id))
        {
            Debug.Log("[GameController] You didnt finish the game so you DIED");
            application.GameManager.SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(application.GameManager.PlayerRemoveFromGame), $"{Networking.LocalPlayer.playerId}");
            application.Player.VRCPlayerApi.TeleportTo(application.GameManager.SpawnPoint.position, Quaternion.identity);
            application.Player.PlayerEffects._Reset();
        }
    }

    // when a player finishes the game
    public virtual void PlayerOutOfGame(string id)
    {
        _currentPlayersInGame.Remove(id);

        for (int i = 0; i < _currentPlayersInGame.Count; i++)
        {
            Debug.Log($"Players: {_currentPlayersInGame[i]}");
        }
        
        if (_currentPlayersInGame.Count == 0) _End();
        
        RequestSerialization();
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
