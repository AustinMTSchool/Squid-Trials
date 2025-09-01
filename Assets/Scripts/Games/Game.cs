
using System;using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class Game : UdonSharpBehaviour
{
    [SerializeField] protected Application application;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected string gameName = "default";
    [SerializeField] protected string description;
    [SerializeField] protected int introTime;
    [SerializeField] protected int endingTime = 10;
    [SerializeField] protected GameController gameController;
    protected readonly string DefaultGame = "main";
    
    [UdonSynced] protected GameState state = 0;

    public GameState State => state;
    protected bool _forceEnd = false;
    public string GameName => gameName;
    public GameController GameController => gameController;
    public Transform SpawnPoint => spawnPoint;
    
    public void SpawnPlayer()
    {
        Debug.Log("Spawning player " + application.Player.VRCPlayerApi.displayName);
        application.Player.VRCPlayerApi.TeleportTo(spawnPoint.position, spawnPoint.rotation);
    }

    public virtual void Initialize()
    {
    }

    public virtual void InitializeBeginning()
    {
    }

    public virtual void Stop()
    {
        
    }

    public virtual void EndGame()
    {
        
    }
}

public enum GameState
{
    NONE = 0,
    INTRO = 1,
    ACTIVE = 2,
    ENDING = 3
}
