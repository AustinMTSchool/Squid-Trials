
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PointStat : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI display;

    [UdonSynced] private int _points = 0;

    public int Points => _points;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _points.ToString();
        RequestSerialization();
    }

    public void AddPoints(int amount)
    {
        _points += amount;
        display.text = _points.ToString();
        RequestSerialization();
    }
    
    public void RemovePoints(int amount)
    {
        _points -= amount;
        display.text = _points.ToString();
        RequestSerialization();
    }
    
    public override void OnDeserialization()
    {
        display.text = _points.ToString();
    }
}
