
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WinStat : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private PointStat pointStat;

    [UdonSynced] private int _wins = 0;

    public int Wins => _wins;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _wins.ToString();
        RequestSerialization();
    }

    public void AddWins(int amount)
    {
        _wins += amount;
        display.text = _wins.ToString();
        RequestSerialization();
    }
    
    public void RemovePoints(int amount)
    {
        _wins -= amount;
        display.text = _wins.ToString();
        RequestSerialization();
    }
    
    public override void OnDeserialization()
    {
        display.text = _wins.ToString();
    }
}