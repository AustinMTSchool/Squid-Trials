
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class EventTesting : UdonSharpBehaviour
{
    [SerializeField] private Player player;

    [UdonSynced] private int count;

    private void Start()
    {
        // IncCount(); For RequestSerialization testing
    }

    public void IncCount()
    {
        count++;
        RequestSerialization();
        SendCustomEventDelayedSeconds(nameof(IncCount), 0.1F);
    }

    public override void OnDeserialization(DeserializationResult result)
    {
        var time = Time.realtimeSinceStartup - result.sendTime;
        Debug.Log(count + "Send Time: " + result.sendTime + ", Recieved: " + result.receiveTime + ", Realtime: " + time);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log("Added a player level");
        this.player.PlayerStat.LevelStat.AddLevel(1);
    }
}
