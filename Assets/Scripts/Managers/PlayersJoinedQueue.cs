
using TMPro;
using UdonSharp;
using UnityEngine;
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

    private int _amountOfPlayers;
    
    [UdonSynced] private int _playersReady;
    [UdonSynced] private bool _canTimerStart = true;
    [UdonSynced] private bool _isTimerActive = false;
    [UdonSynced] private int _timerSeconds = 40;
    
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        _amountOfPlayers = VRCPlayerApi.GetPlayerCount();
        UpdatePlayerCount();
        
        // If we're the master and timer should be running, make sure we own it
        if (Networking.LocalPlayer.isMaster && _isTimerActive && !Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        _amountOfPlayers = VRCPlayerApi.GetPlayerCount();
        UpdatePlayerCount();
        
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) 
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            
        _playersReady++;
        UpdatePlayerReadyCount();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
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
        _timerSeconds = 40;
        UpdateTimerNetwork();
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        if (!_isTimerActive) return;
       
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
    }

    public override void OnDeserialization()
    {
        UpdatePlayerReadyCount();
        UpdateTimer();
    }

    public void UpdateTimerNetwork()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject) || !_isTimerActive)
            return;
        
        tickSound.Play();
        if (_timerSeconds > 0)
        {
            _timerSeconds--;
            UpdateTimer();
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(UpdateTimerNetwork), 1);
        }
        else
        {
            _isTimerActive = false;
            RequestSerialization();
            // Games begin
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
        timerText.text = $"{_timerSeconds}";
    }
}