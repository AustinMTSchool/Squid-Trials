
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class MainGame : Game
{
    [Space, Header("Main Game")]
    [SerializeField] private int beginningTime = 60;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI beginningTextDisplay;

    private DataDictionary _beginningDialogue = new DataDictionary()
    {
        { 55, "Welcome to the squid trials. You will be competing against other players for a prize pool of." },
        { 45, "There are several different games ranging from different difficulties depending on the players left"},
        { 35, "The games will continue to keep going until 1 player is the last one standing"},
        { 25, "You can use classes in the hub to change your playstyle to your liking"},
        { 15, "Items are also a good way to gain some advantages but you will lose them after dying"},
        { 5, "Good luck and enjoy the games!"}
    };
    
    [UdonSynced] private int _currentBeginningTime = 60;
    [UdonSynced] private bool _isBeginningTimeTicking = false;
    [UdonSynced] private bool _startNextGame = false;
    
    public override void InitializeBeginning()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        
        base.InitializeBeginning();
        
        _currentBeginningTime = beginningTime;
        _isBeginningTimeTicking = true;
        SetProgramVariable(nameof(_currentBeginningTime), beginningTime);
        OnBeginningTimerUpdateNetwork();
        RequestSerialization();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        Debug.Log("You are the new owner, begin");
        
        OnBeginningTimerUpdateNetwork();
        RequestSerialization();
    }

    public void OnBeginningTimerUpdateNetwork()
    {
        if (_currentBeginningTime > 0)
        {
            _currentBeginningTime--;
            UpdateBeginningTimer();
            UpdateBeginningTextDisplay();
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(OnBeginningTimerUpdateNetwork), 1);
        }
        else
        {
            _isBeginningTimeTicking = false;
            var game = application.GameManager.GetRandomGame();
            
            if (application.Player.IsInGames)
            {
                application.GameManager.SpawnPlayerInGame(game.GameName);
            }
            
            application.GameManager.SetCurrentGame(game);
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NextGame));
            application.GameManager.StartGame(game.GameName, true);
            RequestSerialization();
        }
    }

    public void NextGame(string gameName)
    {
        if (application.Player.IsInGames)
        {
            application.GameManager.SpawnPlayerInGame(application.GameManager.GameSelected);
        }
    }

    public override void OnDeserialization()
    {
        if (_isBeginningTimeTicking)
        {
            UpdateBeginningTimer();
            UpdateBeginningTextDisplay();
        }
        
    }

    private void UpdateBeginningTimer()
    {
        timerText.text = $"{_currentBeginningTime}";
    }

    private void UpdateBeginningTextDisplay()
    {
        if (_beginningDialogue.TryGetValue(_currentBeginningTime, out DataToken value))
        {
            beginningTextDisplay.text = value.String;
        }
    }
}
