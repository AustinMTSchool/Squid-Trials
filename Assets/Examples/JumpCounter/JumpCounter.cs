using TMPro;
using UdonSharp;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon.Common;

public class JumpCounter : UdonSharpBehaviour
{
    public TextMeshProUGUI jumpText;
    private const string JumpsKey = "jumps";

    private bool _playerRestored;
    
    
    public override void InputJump(bool value, UdonInputEventArgs args)
    {
        if (value && Networking.LocalPlayer.IsPlayerGrounded() && _playerRestored)
        {
            AddJump();
        }
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            _playerRestored = true;
            UpdateTextComponent();
        }
    }

    public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
    {
        if (player.isLocal)
        {
            UpdateTextComponent();
        }
    }

    private void AddJump()
    {
        var currentJumps = PlayerData.GetInt(Networking.LocalPlayer, JumpsKey);
        PlayerData.SetInt("jumps", currentJumps + 1);
    }
    
    private void UpdateTextComponent()  
    {  
        jumpText.text = $"Jumps: {PlayerData.GetInt(Networking.LocalPlayer, JumpsKey)}";    
    }
}
