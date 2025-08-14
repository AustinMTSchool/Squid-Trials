
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Title : UdonSharpBehaviour
{
    [SerializeField] private Application application;
    [SerializeField] private string playerName;
    private string _playerTitle;

    private void Start()
    {
        _playerTitle = name;
        CoroutineFollower();
    }

    public void CoroutineFollower()
    {
        if (!Utilities.IsValid(application.Player.VRCPlayerApi))
        {
            SendCustomEventDelayedSeconds(nameof(CoroutineFollower), 0.1F);
            return;
        }
        
        SendCustomEventDelayedSeconds(nameof(CoroutineFollower), 0.1F);
    }
}
