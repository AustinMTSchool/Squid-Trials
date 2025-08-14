
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerHeightManager : UdonSharpBehaviour
{
    [SerializeField] private float height;
    
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        // TODO actually tes this
        player.SetAvatarEyeHeightByMeters(height);
        player.SetAvatarEyeHeightMinimumByMeters(height);
        player.SetAvatarEyeHeightMaximumByMeters(height);
    }
}
