
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameManager : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Game[] allGames;
    [SerializeField] private Game mainGame;
    [UdonSynced] private string _gameMode = "lobby";
    [UdonSynced] private string _gameSelected = "";
    
    private System.Random random = new System.Random();
    private DataDictionary _allGames = new DataDictionary();
    
    public string Mode => _gameMode;
    public string GameSelected => _gameSelected;

    private void Start()
    {
        random = new System.Random();
        foreach (var game in allGames)
        {
            _allGames.Add(game.GameName, new DataToken(game));
        }
    }

    public void ResetAllGames()
    {
        foreach (var game in allGames)
        {
            if (_allGames.ContainsKey(game.GameName)) continue;
            _allGames.Add(game.GameName, new DataToken(game));
        }
    }

    public void SetInLobby(string mode)
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        
        _gameMode = mode;
        RequestSerialization();
    }

    public void StartGame(string gameName, bool remove = false)
    {
        if (_allGames.TryGetValue(gameName, TokenType.Reference, out var gameToken))
        {
            Game game = (Game) gameToken.Reference;
            Debug.Log("Beginning game: " + game.GameName);
            game.Initialize();
            if (remove) _allGames.Remove(game.GameName);
        }
    }

    public void StartMainGame(bool initBeginning = false)
    {
        if (initBeginning)
        {
            mainGame.InitializeBeginning();
        }
        else
        {
            mainGame.Initialize();
        }
    }

    public void SpawnPlayerInGame(string gameName)
    {
        if (_allGames.TryGetValue(gameName, TokenType.Reference, out var gameToken))
        {
            Game game = (Game) gameToken.Reference;
            game.SpawnPlayer();
        }
    }

    public Game GetRandomGame()
    {
        if (_allGames.Count == 0) ResetAllGames();

        DataList values = _allGames.GetValues();
        int randomIndex = random.Next(0, values.Count);
        DataToken token = values[randomIndex];
        return (Game) token.Reference;
    }

    public void SetCurrentGame(Game game)
    {
        _gameSelected = game.GameName;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
    }
}
