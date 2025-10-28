
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class LevelStat : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers manager;
    [SerializeField] private TextMeshProUGUI display;

    [UdonSynced] private int _level = 1;
    [UdonSynced] private int _experience = 0;

    public int Level => _level;
    public int Experience => _experience;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _level.ToString();
        RequestSerialization();
    }

    public void AddExperience(int amount)
    {
        _experience += amount;
        RequestSerialization();
    }

    public void AddLevel(int amount)
    {
        _level += amount;
        display.text = _level.ToString();
        RequestSerialization();
        manager.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(manager.UpdateSlotPosition), Networking.LocalPlayer.playerId, _level);
    }

    public void RemoveLevel(int amount)
    {
        _level -= amount;
        display.text = _level.ToString();
        RequestSerialization();
        manager.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(manager.UpdateSlotPosition), Networking.LocalPlayer.playerId, _level);
    }

    public override void OnDeserialization()
    {
        display.text = _level.ToString();
    }
}
