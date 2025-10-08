
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GlassBridgeGame : Game
{
    [SerializeField] private GlassBreakerManager _glassBreakerManager;
    
    private GlassBridgeController _gbController;
    private string _broken_pieces = "<broken_pieces>";
    
    private void Start()
    {
        if (Utilities.IsValid(gameController))
        {
            _gbController = (GlassBridgeController) gameController;
        }
    }
    
    public override void _OnOutroTick()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        base._OnOutroTick();

        if (_currentOutroTIme <= 0)
        {
            _gbController.GlassBreakerManager._Reset();
        }
    }

    protected override void _UpdateIntroDisplayText()
    {
        base._UpdateIntroDisplayText();
        if (introDisplay.text.Contains(_broken_pieces))
        {
            introDisplay.text = introDisplay.text.Replace(_broken_pieces, _glassBreakerManager._GetPossiblePieceBreakables() + "");
        }
    }
}