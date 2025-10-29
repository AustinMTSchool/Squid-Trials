
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KillStat : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private PointStat pointStat;

    [UdonSynced] private int _kills = 0;

    public int Kills => _kills;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _kills.ToString();
        RequestSerialization();
    }

    public void AddKills(int amount)
    {
        _kills += amount;
        display.text = _kills.ToString();
        pointStat.AddPoints(150);
        RequestSerialization();
    }
    
    public override void OnDeserialization()
    {
        display.text = _kills.ToString();
    }
}
