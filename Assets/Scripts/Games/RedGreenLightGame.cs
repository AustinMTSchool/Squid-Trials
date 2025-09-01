
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.Ocsp;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RedGreenLightGame : Game
{
    [Space, Header("Red Light Green Light")]
    [SerializeField] private TextMeshProUGUI introDisplay;
    
    private DataDictionary _introDic = new DataDictionary()
    {
           {15, "You will have a set amount of time to get to the end of the game. You can move on green, but must be still on red"},
           {7, "If you do not finish fast enough or break the rules you will be eliminated"}
    };
    
    [UdonSynced] private bool _isIntro = false;
    [UdonSynced] private int _currentIntroTime;
    [UdonSynced] private bool _isEnding = false;
    [UdonSynced] private int _currentEndingTime;
    
    public override void Initialize()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        Debug.Log("Game Initialize");
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        base.Initialize();
        _currentIntroTime = introTime;
        _isIntro = true;
        _forceEnd = false;
        gameController.AssignCurrentPlayersInGame();
        state = GameState.INTRO;
        
        OnIntroNetwork();
        RequestSerialization();
    }

    public void OnIntroNetwork()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        Debug.Log("MASTER");

        if (_forceEnd)
        {
            Debug.Log("Force Ended intro");
            _currentIntroTime = 0;
            _isIntro = false;
            state = GameState.NONE;
            RequestSerialization();
            return;
        }

        if (_isIntro && _currentIntroTime > 0)
        {
            Debug.Log("is intro and current intro time above 0");
            _currentIntroTime--;
            Debug.Log("Time: " + _currentIntroTime);
            UpdateIntroDisplayText();
            RequestSerialization();
            SendCustomEventDelayedSeconds(nameof(OnIntroNetwork), 1F);
        }
        else
        {
            _isIntro = false;
            state = GameState.ACTIVE;
            Debug.Log("Game State: " + state);
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(BeginGame));
        }
    }

    [NetworkCallable]
    public void BeginGame()
    {
        gameController.Begin();
    }

    public override void EndGame()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        Debug.Log("EndGame Proceeded");
        state = GameState.ENDING;
        _isEnding = true;
        _currentEndingTime = endingTime;
        RequestSerialization();
        
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(EndGameNetwork));
    }

    [NetworkCallable]
    public void EndGameNetwork()
    {
        if (!Networking.LocalPlayer.isMaster) return;

        if (_forceEnd)
        {
            Debug.Log("Force Ended ending game");
            state = GameState.NONE;
            _isEnding = false;
            _currentEndingTime = 0;
            RequestSerialization();
            return;
        }
        
        _currentEndingTime--;
        if (_currentEndingTime <= 0)
        {
            state = GameState.NONE;
            _isEnding = false;
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(DefaultGameNetwork));
            application.GameManager.StartMainGame();
        }
        else
        {
            Debug.Log("Ending Game");
            // wait set amount of time to end game
            SendCustomEventDelayedSeconds(nameof(EndGameNetwork), 1F);
        }
    }

    [NetworkCallable]
    public void DefaultGameNetwork()
    {
        if (application.Player.IsInGames)
        {
            application.GameManager.SpawnPlayerInGame(DefaultGame);
        }
    }

    public override void Stop()
    {
        _forceEnd = true;
        gameController.End();
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.isLocal) return;
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        if (_isIntro)
        {
            OnIntroNetwork();
        }

        if (_isEnding)
        {
            EndGameNetwork();
        }
    }

    public override void OnDeserialization()
    {
        if (_isIntro)
        {
            UpdateIntroDisplayText();
        }
    }

    private void UpdateIntroDisplayText()
    {
        if (_introDic.TryGetValue(_currentIntroTime, out var value))
        {
            var intro = value.String;
            introDisplay.text = intro;
        }
    }
}
