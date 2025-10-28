
using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using VRC.SDKBase;
using VRC.Udon;

public class EventTesting : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log("Added a player level");
        this.player.PlayerStat.LevelStat.AddLevel(1);
    }
}
