
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GlassBridgeController : GameController
{
    [SerializeField] private GlassBreakerManager glassBreakerManager;
    [SerializeField] private GameObject barrier;
    
    public GlassBreakerManager GlassBreakerManager => glassBreakerManager;
    
    public override void _Begin()
    {
        base._Begin();

        barrier.SetActive(false);
        
        if (!Networking.LocalPlayer.isMaster) return;
        
        glassBreakerManager._Initialize();
    }


    public override void _End()
    {
        if (!Networking.LocalPlayer.isMaster) return;
        
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EndLocalSide));
        RequestSerialization();
        
        Debug.Log("gb._End => _End");
        base._End();
    }
    
    [NetworkCallable]
    public void EndLocalSide()
    {
        barrier.SetActive(true);
    }
}
