
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

    public string GameName => gameName;
    
    public void SpawnPlayer()
    {
        application.Player.VRCPlayerApi.TeleportTo(spawnPoint.position, spawnPoint.rotation);
    }

    public virtual void Initialize()
    {
        SpawnPlayer();
    }

    public virtual void InitializeBeginning()
    {
        SpawnPlayer();
    }
}
