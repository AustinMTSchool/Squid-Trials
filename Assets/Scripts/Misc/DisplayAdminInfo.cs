
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class DisplayAdminInfo : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI isMaster;
    [SerializeField] private TextMeshProUGUI instanceOwner;

    private void Start()
    {
        if (Networking.LocalPlayer.isMaster)
            isMaster.text = $"Master: True";
        else
            isMaster.text = $"Master: False";
        
        if (Networking.LocalPlayer.isInstanceOwner)
            instanceOwner.text = $"Instance Owner: True";
        else
            instanceOwner.text = $"Instance Owner: False";
    }

    public override void OnMasterTransferred(VRCPlayerApi newMaster)
    {
        if (newMaster.isLocal)
            isMaster.text = $"Master: True";
        else
            isMaster.text = $"Master: False";
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        if (player.isInstanceOwner)
            instanceOwner.text = $"Instance Owner: True";
        else
            instanceOwner.text = $"Instance Owner: False";
    }
}
