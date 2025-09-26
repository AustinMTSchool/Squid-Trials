
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GlassBridgeGame : Game
{
    
    private GlassBridgeController _gbController;
    
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
}