
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GameManager : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Game[] allGames;
    [SerializeField] private Game mainGame;
    [SerializeField] private PlayersJoinedQueue playersJoinedQueue;
    
    private DataList _allPlayersInGame = new DataList();
    
    [UdonSynced] private string _gameMode = "lobby";
    [UdonSynced] private string _jsonAvailableGames;
    [UdonSynced] private string _jsonAllPlayersInGame;
    [UdonSynced] private bool _isGamesActive;
    [UdonSynced] private string _currentGame;
    
    private System.Random random = new System.Random();
    private DataList _availableGames = new DataList();
    private DataDictionary _allGamesByName = new DataDictionary();
    
    public string Mode => _gameMode;

    private void Start()
    {
        random = new System.Random();
        foreach (var game in allGames)
        {
            _allGamesByName.Add(game.GameName, new DataToken(game));
        }
        _allGamesByName.Add(mainGame.GameName, new DataToken(mainGame));
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (!newMaster.IsOwner(gameObject))
            Networking.SetOwner(newMaster, gameObject);
    }

    public void ResetAllGames()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        foreach (var game in allGames)
        {
            if (_availableGames.Contains(game.GameName)) continue;
            _availableGames.Add(game.GameName);
            RequestSerialization();
        }
    }

    public void SetInLobby(string mode)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        _gameMode = mode;
        RequestSerialization();
    }

    public void StartGame(string gameName, bool remove = false)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        if (_availableGames.Count == 0)
            ResetAllGames();
        
        if (_availableGames.Contains(gameName))
        {
            if (_allGamesByName.TryGetValue(gameName, out var gameToken))
            {
                Game game = (Game) gameToken.Reference;
                Debug.Log("Beginning game: " + game.GameName);
                game.Initialize();
                _currentGame = game.GameName;
                if (remove)
                {
                    _availableGames.Remove(gameName);
                }
                RequestSerialization();
            }
        }
    }

    public void AddPlayersToGames(DataList players)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        for (int i = 0; i < players.Count; i++)
        {
            _allPlayersInGame.Add($"{players[i]}");
        }
        RequestSerialization();
    }

    public void StartMainGame(bool initBeginning = false)
    {
        if (!Networking.LocalPlayer.isMaster) return;
        if (initBeginning)
        {
            mainGame.InitializeBeginning();
            _isGamesActive = true;
        }
        else
        {
            mainGame.Initialize();
        }
        Debug.Log("Setting Current Game to: " + mainGame.GameName);
        _currentGame = mainGame.GameName;
        Debug.Log("Current Game: " +  _currentGame);
        RequestSerialization();
    }

    public void SpawnPlayerInGame(string gameName)
    {
        Debug.Log("Attempting to spawn player in game");

        if (_allGamesByName.TryGetValue(gameName, out var gameToken))
        {
            Game game = (Game) gameToken.Reference;
            game.SpawnPlayer();
        }
    }

    public void SpawnPlayerInMainGame()
    {
        Debug.Log("[GameManager] SpawnPlayerInMainGame");
        mainGame.SpawnPlayer();
    }

    public Game GetRandomGame()
    {
        if (!Networking.LocalPlayer.isMaster) return null;
        if (_availableGames.Count == 0) ResetAllGames();

        int randomIndex = random.Next(0, _availableGames.Count);
        DataToken chosenGameToken = _availableGames[randomIndex];
        string gameName = chosenGameToken.String;
        if (_allGamesByName.TryGetValue(gameName, out var gameToken))
        {
            return (Game) gameToken.Reference;
        }
        else
        {
            return null;
        }
    }

    public override void OnPreSerialization()
    {
        if (VRCJson.TrySerializeToJson(_availableGames, JsonExportType.Minify, out DataToken result))
            _jsonAvailableGames = result.String;
        else
            Debug.LogError(result.ToString());
        
        if (VRCJson.TrySerializeToJson(_allPlayersInGame, JsonExportType.Minify, out DataToken playersInGameList))
            _jsonAllPlayersInGame = playersInGameList.String;
        else
            Debug.LogError(result.ToString());
    }

    public override void OnDeserialization()
    {
        if(VRCJson.TryDeserializeFromJson(_jsonAvailableGames, out DataToken result))
            _availableGames = result.DataList;
        else
            Debug.LogError(result.ToString());
        
        if(VRCJson.TryDeserializeFromJson(_jsonAllPlayersInGame, out DataToken jsonAllPlayersInGame))
            _allPlayersInGame = jsonAllPlayersInGame.DataList;
        else
            Debug.LogError(result.ToString());
        
        Debug.Log("Curremt Game: " +  _currentGame);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        Debug.Log($"Player left: {player.playerId}");
        
        if (!Networking.LocalPlayer.isMaster) return;
        if (!_allPlayersInGame.Contains($"{player.playerId}")) return;

        _allPlayersInGame.Remove($"{player.playerId}");

        if (_allPlayersInGame.Count <= 0)
        {
            Debug.Log($"Games are ending");
            EndGames();
        }

        RequestSerialization();
    }

    public override void OnPlayerRespawn(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        if (!this.player.IsInGames) return;

        Debug.Log($"{player.playerId} was in game sending event to {Networking.Master.playerId}");
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(PlayerRemoveFromGame), $"{player.playerId}");
        
        this.player.SetInGames(false);
        RequestSerialization();
    }

    [NetworkCallable]
    public void PlayerRemoveFromGame(string playerId)
    {
        if (!Networking.LocalPlayer.isMaster) return;
            
        _allPlayersInGame.Remove($"{playerId}");
        RequestSerialization();
        
        if (_allPlayersInGame.Count <= 0)
        {
            EndGames();
        }
    }

    public void EndGames()
    {
        Debug.Log("Games is ending for " + _currentGame);
        if (!Networking.LocalPlayer.isMaster) return;

        if (_allGamesByName.TryGetValue(_currentGame, out var gameToken))
        {
            Game game = (Game)gameToken.Reference;
            Debug.Log("Ending Game: " + game.GameName);
            game.End();
        }
        else
        {
            Debug.Log("current game was not found");
        }

        _isGamesActive = false;
        _gameMode = "lobby";
        ResetAllGames();
        playersJoinedQueue.SetCanTimerStart(true);
        RequestSerialization();
    }
}
