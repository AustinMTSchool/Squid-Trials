
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Game : UdonSharpBehaviour
{
    [SerializeField] private Application application;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string gameName = "default";
    [SerializeField] private string description;

    public string GameName => gameName;
    
    public void SpawnPlayer()
    {
        application.Player.VRCPlayerApi.TeleportTo(spawnPoint.position, spawnPoint.rotation);
    }

    public void Initialize()
    {
        SpawnPlayer();
    }
}
