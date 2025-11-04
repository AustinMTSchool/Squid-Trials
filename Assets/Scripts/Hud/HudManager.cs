
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HudManager : UdonSharpBehaviour
{
    [SerializeField] private HudVR hudVR;
    [SerializeField] private HudPC hudPC;

    private Hud _hud;
    
    public Hud Hud => _hud;

    private void Start()
    {
        if (Networking.LocalPlayer.IsUserInVR())
        {
            hudVR.gameObject.SetActive(true);
            _hud = hudVR;
        }
        else
        {
            hudPC.gameObject.SetActive(true);
            _hud = hudPC;
        }
    }
}
