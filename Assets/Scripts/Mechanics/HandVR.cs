
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HandVR : Hand
{
    [SerializeField] private Transform hand;
    [SerializeField] private VRCPlayerApi.TrackingDataType type;

    protected override void Start()
    {
        _playerApi = Networking.LocalPlayer;
    }

    private void LateUpdate()
    {
        VRCPlayerApi.TrackingData data = _playerApi.GetTrackingData(type);
        hand.position = data.position;

    }
}
