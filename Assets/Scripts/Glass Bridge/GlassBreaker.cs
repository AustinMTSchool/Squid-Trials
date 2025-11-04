
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class GlassBreaker : UdonSharpBehaviour
{
    [SerializeField, UdonSynced] private bool breakable = false;
    [SerializeField] private Player player;
    
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log("Player trigger enter : breakable " + breakable);
        if (!player.isLocal) return;
        
        if (breakable)
        {
            breakable = false;
            RequestSerialization();
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Break));
        }
    }

    public void _SetBreakState(bool state)
    {
        Debug.Log("State set on " + gameObject.name + " is set to " + state);
        breakable = state;
        RequestSerialization();
    }
    
    [NetworkCallable]
    public void Reset()
    {
        gameObject.SetActive(true);

        if (Networking.LocalPlayer.isMaster)
        {
            breakable = false;
            RequestSerialization();
        }
    }
    
    [NetworkCallable]
    public void Break()
    {
        gameObject.SetActive(false);
    }
}
