
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PlayersJoinedQueue : UdonSharpBehaviour
{
    [SerializeField] private Application application;
    [SerializeField] private TextMeshProUGUI allPlayersText;
    [SerializeField] private TextMeshProUGUI playersReadyText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Audio tickSound;
    [SerializeField] private Game beginningGame;

    private int _amountOfPlayers;
    private DataList _queuedPlayers = new DataList();
    
    [UdonSynced] private string _queuedPlayersJSON;
    [UdonSynced] private int _playersReady;
    [UdonSynced] private bool _canTimerStart = true;
    [UdonSynced] private bool _isTimerActive = false;
    [UdonSynced, SerializeField] private int timerSeconds = 20;
    
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        _amountOfPlayers = VRCPlayerApi.GetPlayerCount();
        UpdatePlayerCount();
        
        if (Networking.LocalPlayer.isMaster && _isTimerActive && !Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        _amountOfPlayers--;
        UpdatePlayerCount();

        Debug.Log("Master is updating player left info");
        if (_queuedPlayers.Contains(player.displayName))
        {
            Debug.Log("Removed a player from queue");
            _queuedPlayers.Remove(player.displayName);
            Debug.Log($"{_playersReady}");
            _playersReady--;
            Debug.Log($"{_playersReady}");
            UpdatePlayerReadyCount();
        }
        RequestSerialization();
    }
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (player.isLocal) 
            application.Player.SetPlayerInQueue(true);
        
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (!Networking.LocalPlayer.IsOwner(gameObject)) 
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            
        _playersReady++;
        UpdatePlayerReadyCount();
        _queuedPlayers.Add(player.displayName);
        RequestSerialization();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (player.isLocal)
            application.Player.SetPlayerInQueue(false);
        
        if (Networking.LocalPlayer.isMaster)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            
            _playersReady--;
            UpdatePlayerReadyCount();
        }
    }

    public void OnClick()
    {
        if (!Networking.LocalPlayer.isMaster) return;

        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        if (!_canTimerStart) return;
        if (application.GameManager.Mode != "lobby") return;

        _canTimerStart = false;
        _isTimerActive = true;
        timerSeconds = 20;
        UpdateTimerNetwork();
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        if (!_isTimerActive) return;
       
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
    }
    
    public override void OnPreSerialization()
    {
        if (VRCJson.TrySerializeToJson(_queuedPlayers, JsonExportType.Minify, out DataToken result))
        {
            _queuedPlayersJSON = result.String;
        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }

    public override void OnDeserialization()
    {
        if(VRCJson.TryDeserializeFromJson(_queuedPlayersJSON, out DataToken result))
        {
            _queuedPlayers = result.DataList;
        }
        else
        {
            Debug.LogError(result.ToString());
        }
        
        UpdatePlayerReadyCount();
        UpdateTimer();
        UpdatePlayerReadyCount();
    }

    public void UpdateTimerNetwork()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject) || !_isTimerActive)
            return;
        
        tickSound.Play();
        if (timerSeconds > 0)
        {
            timerSeconds--;
            UpdateTimer();
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
        }
        else
        {
            // begin game
            _isTimerActive = false;
            application.GameManager.SetInLobby("game");
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(BeginGame));
            application.GameManager.StartMainGame(true);
        }
    }

    public void BeginGame()
    {
        if (application.Player.IsPlayerInQueue)
        {
            Debug.Log($"Added to game (was in queue)");
            application.Player.SetInGames(true);
            application.GameManager.SpawnPlayerInGame(beginningGame.GameName);
        }
        else
        {
            Debug.Log($"Was not in queue");
        }
    }

    private void UpdatePlayerCount()
    {
        allPlayersText.text = $"{_amountOfPlayers}";
    }

    private void UpdatePlayerReadyCount()
    {
        playersReadyText.text = $"{_playersReady}";
    }
    
    private void UpdateTimer()
    {
        tickSound.Play();
        timerText.text = $"{timerSeconds}";
    }
}