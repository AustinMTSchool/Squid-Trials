
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Game : UdonSharpBehaviour
{
    [SerializeField] protected Application application;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected string gameName = "default";
    [SerializeField] protected string description;
    [SerializeField] protected int introTime;

    [UdonSynced] protected int _currentIntroTime;

    protected bool _forceEnd = false;

    public string GameName => gameName;
    
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

    public virtual void End()
    {
        
    }
}
