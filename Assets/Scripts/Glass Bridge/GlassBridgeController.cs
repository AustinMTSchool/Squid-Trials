
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GlassBridgeController : GameController
{

    [SerializeField] private GlassBreakerManager glassBreakerManager;
    
    public GlassBreakerManager GlassBreakerManager => glassBreakerManager;
    
    public override void _Begin()
    {
        base._Begin();

        if (!Networking.LocalPlayer.isMaster) return;
        glassBreakerManager._Initialize();
        Debug.Log("BEGIN");
    }
    
}
