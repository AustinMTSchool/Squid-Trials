
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GameManager : UdonSharpBehaviour
{
    [UdonSynced] private string _gameMode = "lobby";
    
    public string Mode => _gameMode;

    public void SetInLobby(string mode)
    {
        _gameMode = mode;
    }
    
}
