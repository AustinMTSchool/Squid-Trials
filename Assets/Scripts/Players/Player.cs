
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class Player : UdonSharpBehaviour
{
    private VRCPlayerApi _vrcPlayer;
    // When the player snaps a photo
    private bool _isPlayerRegistered;
    private bool _isPersistenceRestored;
    
    public bool IsPlayerRegistered => _isPlayerRegistered;
    public bool IsPersistenceRestored => _isPersistenceRestored;
    
    public VRCPlayerApi VRCPlayerApi => _vrcPlayer;
    private void Start()
    {
        _vrcPlayer = Networking.LocalPlayer;
    }
    
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        if (!PlayerData.HasKey(player, "skill_rating"))
        { 
            PlayerData.SetFloat("skill_rating", 1.0F);
        }
        
        if (!PlayerData.HasKey(player, "level")) 
        { 
            PlayerData.SetInt("level", 1);
        }
        
        if (!PlayerData.HasKey(player, "wins")) 
        { 
            PlayerData.SetInt("wins", 0);
        }

        if (!PlayerData.HasKey(player, "kills"))
        {
            PlayerData.SetInt("kills", 0);
        }
        
        if (!PlayerData.HasKey(player, "points"))
        {
            PlayerData.SetInt("points", 0);
        }
    }

    public string GetDisplayName()
    {
        return _vrcPlayer.displayName;
    }

    public void SetPlayerRegistered(bool value)
    {
        _isPlayerRegistered = value;
    }
}
