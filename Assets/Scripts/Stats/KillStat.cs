
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class KillStat : UdonSharpBehaviour
{
    [SerializeField] private KillsAchievement killsAchievement;
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private PointStat pointStat;
    [SerializeField] private LevelStat levelStat;

    [UdonSynced] private int _kills = 0;

    public int Kills => _kills;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _kills.ToString();
        killsAchievement._Init(_kills);
        
        RequestSerialization();
    }

    public void AddKills(int amount)
    {
        _kills += amount;
        display.text = _kills.ToString();
        pointStat.AddPoints(150);
        levelStat.AddExperience(800);
        killsAchievement._UpdateCompletion(_kills);
        RequestSerialization();
    }
    
    public override void OnDeserialization()
    {
        display.text = _kills.ToString();
    }
}
