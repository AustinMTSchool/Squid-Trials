
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GameController : UdonSharpBehaviour
{
    [SerializeField] protected GameManager gameManager;
    
    protected DataList _currentPlayersInGame = new DataList();
    
    [UdonSynced] protected string _jsonCurrentPlayersInGame;
    
    public DataList CurrentPlayersInGame => _currentPlayersInGame;

    public virtual void Intro()
    {
        
    }
    
    public virtual void Begin()
    {
        
    }

    public virtual void End()
    {
        
    }

    public virtual void Stop()
    {
        
    }

    public virtual void PlayerOutOfGame(string id)
    {
        _currentPlayersInGame.Remove(id);

        for (int i = 0; i < _currentPlayersInGame.Count; i++)
        {
            Debug.Log($"Players: {_currentPlayersInGame[i]}");
        }
        
        RequestSerialization();
    }

    public virtual void PlayerActiveGame(string id)
    {
        if (_currentPlayersInGame.Contains(id)) return;
        
        _currentPlayersInGame.Add(id);
        RequestSerialization();
    }
    
    public override void OnPreSerialization()
    {
        if (VRCJson.TrySerializeToJson(_currentPlayersInGame, JsonExportType.Minify, out DataToken allPlayers))
            _jsonCurrentPlayersInGame = allPlayers.String;
        else
            Debug.LogError(allPlayers.ToString());
    }

    public override void OnDeserialization()
    {
        if(VRCJson.TryDeserializeFromJson(_jsonCurrentPlayersInGame, out DataToken jsonCurrentPlayersInGame))
            _currentPlayersInGame = jsonCurrentPlayersInGame.DataList;
        else
            Debug.LogError(jsonCurrentPlayersInGame.ToString());
    }

    public void AssignCurrentPlayersInGame()
    {
        _currentPlayersInGame = gameManager.AllPlayersInGame.DeepClone();
    }
}
