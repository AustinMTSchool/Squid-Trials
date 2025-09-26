
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Lobby : UdonSharpBehaviour
{
    [Space, Header("Main Game")]
    [SerializeField] private int tutorialTime = 60;
    [SerializeField] private int periodTime = 20;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI announcementText;
    [SerializeField] protected Application application;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected Dialogue tutorialDialogue;

    [UdonSynced] private int _currentTutorialTime = 60;
    [UdonSynced] private bool _isTutorialTimeTicking = false;
    [UdonSynced] private int _currentPeriodTime = 20;
    [UdonSynced] private bool _isPeriodTimeTicking = false;
    [UdonSynced] private bool _forceEnd = false;

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.isLocal) return;
        
        if (_isTutorialTimeTicking) 
            _OnTutorialTick();
        
        if (_isPeriodTimeTicking)
            _OnPeriodTimeTick();
    }

    public override void OnDeserialization()
    {
        if (_isTutorialTimeTicking)
        {
            _UpdateTimer(_currentTutorialTime);
            _UpdateTutorialTextDisplay();
        }

        if (_isPeriodTimeTicking)
        {
            _UpdateTimer(_currentPeriodTime);
            _UpdateTutorialTextDisplay();
        }
    }

    public void _SpawnPlayer()
    {
        application.Player.VRCPlayerApi.TeleportTo(spawnPoint.position, spawnPoint.rotation);
    }
    
    public void _Begin(bool tutortial = false)
    {
        if (!Networking.LocalPlayer.isMaster) return;

        if (tutortial)
        {
            _currentTutorialTime = tutorialTime;
            _isTutorialTimeTicking = true;
            _OnTutorialTick();
        }
        else
        {
            _currentPeriodTime = periodTime;
            _isPeriodTimeTicking = true;
            _OnPeriodTimeTick();
        }
        RequestSerialization();
    }

    public void _Stop()
    {
        Debug.Log("[Lobby] Stopping games.");
        _forceEnd = true;
        RequestSerialization();
    }

    public void _OnTutorialTick()
    {
        if (_forceEnd)
        {
            _forceEnd = false;
            Debug.Log($"[{nameof(Lobby)}] Force ending tutorial phase.");
            return;
        }

        if (_currentTutorialTime > 0)
        {
            _currentTutorialTime--;
            _UpdateTimer(_currentTutorialTime);
            _UpdateTutorialTextDisplay();
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(_OnTutorialTick), 1);
        }
        else
        {
            Debug.Log("[Lobby] Starting next game");
            _isTutorialTimeTicking = false;
            Game game = application.GameManager.GetRandomGame();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(SpawnPlayerNextGame), game.GameName);
            application.GameManager.StartGame(game.GameName, true);
            RequestSerialization();
        }
    }

    public void _OnPeriodTimeTick()
    {
        if (_forceEnd)
        {
            _forceEnd = false;
            Debug.Log($"[{nameof(Lobby)}] Force ending period phase.");
            return;
        }

        if (_currentPeriodTime > 0)
        {
            _currentPeriodTime--;
            _UpdateTimer(_currentPeriodTime);
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(_OnPeriodTimeTick), 1);
        }
        else
        {
            _isPeriodTimeTicking = false;
            Game game = application.GameManager.GetRandomGame();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(SpawnPlayerNextGame), game.GameName);
            application.GameManager.StartGame(game.GameName, true);
            RequestSerialization();
        }
    }

    [NetworkCallable]
    public void SpawnPlayerNextGame(string gameName)
    {
        if (application.Player.IsInGames)
        {
            application.GameManager.SpawnPlayerInGame(gameName);
        }
    }

    private void _UpdateTimer(int time)
    {
        timerText.text = $"{time}";
    }

    private void _UpdateTutorialTextDisplay()
    {
        if (tutorialDialogue.DialogueList.TryGetValue(_currentTutorialTime, out DataToken value))
        {
            announcementText.text = value.String;
        }
    }
}
