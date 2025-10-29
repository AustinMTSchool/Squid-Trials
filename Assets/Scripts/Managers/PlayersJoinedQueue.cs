
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
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

    private DataList _queuedPlayers = new DataList();
    
    [UdonSynced] private int _amountOfPlayers;
    [UdonSynced] private string _queuedPlayersJSON;
    [UdonSynced] private int _playersReady;
    [UdonSynced] private bool _canTimerStart = true;
    [UdonSynced] private bool _isTimerActive = false;
    [UdonSynced, SerializeField] private int timerSeconds = 20;
    
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Debug.Log($"[PlayersJoinedQueue] OnPlayerJoined");
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
        
        Debug.Log($"[PlayersJoinedQueue] OnPlayerLeft");
        _amountOfPlayers--;
        UpdatePlayerCount();
        
        if (_queuedPlayers.Contains($"{player.playerId}"))
        {
            _queuedPlayers.Remove($"{player.playerId}");
            _playersReady--;
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
        
        Debug.Log($"[PlayersJoinedQueue] OnPlayerTriggerEnter");
        _playersReady++;
        UpdatePlayerReadyCount();
        _queuedPlayers.Add($"{player.playerId}");
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
            Debug.Log($"[PlayersJoinedQueue] OnPlayerTriggerExit");
            _playersReady--;
            UpdatePlayerReadyCount();
            _queuedPlayers.Remove($"{player.playerId}");
            Debug.Log("updated for master leave queue " + _playersReady);
            RequestSerialization();
        }
    }

    public void OnClick()
    {
        if (!Networking.LocalPlayer.isMaster) return;

        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        if (!_canTimerStart) return;
        if (application.GameManager.Mode != "hub") return;

        Debug.Log($"[PlayersJoinedQueue] OnClick");
        _canTimerStart = false;
        _isTimerActive = true;
        timerSeconds = 20;
        UpdateTimerNetwork();
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        if (!_isTimerActive) return;
       
        Debug.Log($"[PlayersJoinedQueue] OnMasterTransferred");
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
        UpdatePlayerCount();
        UpdatePlayerReadyCount();
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
        Debug.Log($"[PlayersJoinedQueue] OnDeserialization");
        if(VRCJson.TryDeserializeFromJson(_queuedPlayersJSON, out DataToken result))
        {
            _queuedPlayers = result.DataList;
        }
        else
        {
            Debug.LogError(result.ToString());
        }
        
        UpdatePlayerCount();
        UpdateTimer();
        UpdatePlayerReadyCount();
    }

    public void UpdateTimerNetwork()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject) || !_isTimerActive)
            return;
        
        Debug.Log($"[PlayersJoinedQueue] UpdateTimerNetwork");
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
            Debug.Log("[PlayersJoinedQueue] BEGINNING GAME");
            if (_playersReady <= application.GameManager.NumberOfWinners)
            {
                _isTimerActive = false;
                _canTimerStart = true;
                timerSeconds = 20;
                Debug.Log("Not enough players ready");
                RequestSerialization();
                return;
            }
            
            _isTimerActive = false;
            application.GameManager.SetInLobby("game");
            RequestSerialization();
            application.GameManager.AddPlayersToGames(_queuedPlayers);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(BeginGame));
            application.GameManager.StartLobby(true);
        }
    }

    [NetworkCallable]
    public void BeginGame()
    {
        Debug.Log($"[PlayersJoinedQueue] BeginGame");
        if (application.Player.IsPlayerInQueue)
        {
            Debug.Log($"Added to game (was in queue) " + Networking.LocalPlayer.playerId);
            application.Player.PlayerStat.GamesPlayedStat.AddGamesPlayed(1);
            application.Player.SetInGames(true);
            application.GameManager.SpawnPlayerInLobby();
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

    public void SetCanTimerStart(bool value)
    {
        _canTimerStart = value;
        RequestSerialization();
    }
}