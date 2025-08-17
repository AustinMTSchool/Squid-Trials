
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameManager : UdonSharpBehaviour
{
    [SerializeField] private Game[] allGames;
    [UdonSynced] private string _gameMode = "lobby";

    private DataDictionary _allGames = new DataDictionary();
    
    public string Mode => _gameMode;

    private void Start()
    {
        foreach (var game in allGames)
        {
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

    public void StartGame(string gameName)
    {
        if (_allGames.TryGetValue(gameName, TokenType.Reference, out var gameToken))
        {
            Game game = (Game) gameToken.Reference;
            game.Initialize();
        }
    }
}
