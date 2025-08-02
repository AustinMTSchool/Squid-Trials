
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerStat : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers statsManagers;
    [SerializeField] private TextMeshProUGUI playerNameDisplay; 
    [SerializeField] private TextMeshProUGUI skillRatingDisplay;
    [SerializeField] private TextMeshProUGUI levelDisplay;
    [SerializeField] private TextMeshProUGUI wonDisplay;
    [SerializeField] private TextMeshProUGUI killDisplay;
    [SerializeField] private TextMeshProUGUI pointsDisplay;
    [SerializeField] private TextMeshProUGUI rankDisplay;

    private int _level = 1;
    private float _skillRating = 1.0F;
    private int _wins = 0;
    private int _kills = 0;
    private int _points = 0;

    private void Start()
    {
        statsManagers.SetupSlot(this);
        playerNameDisplay.text = GetPlayer().displayName;
    }

    public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
    {
        if (!Networking.IsOwner(player, gameObject)) return;
        
        if (PlayerData.TryGetFloat(player, "skill_rating", out float skillRating))
        {
            _skillRating = skillRating;
            skillRatingDisplay.text = $"{_skillRating:2F}";
        }

        if (PlayerData.TryGetInt(player, "wins", out int wins))
        {
            _wins = wins;
            wonDisplay.text = $"{_wins}";
        }

        if (PlayerData.TryGetInt(player, "kills", out int kills))
        {
            _kills = kills;
            killDisplay.text = $"{_kills}";
        }

        if (PlayerData.TryGetInt(player, "points", out int points))
        {
            _points = points;
            pointsDisplay.text = $"{_points}";
        }
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!Networking.IsOwner(player, gameObject)) return;

        if (PlayerData.HasKey(player, "skill_rating"))
        {
            _skillRating = PlayerData.GetFloat(player, "skill_rating");
            skillRatingDisplay.text = $"{_skillRating:2F}";
        }
        else
        {
            PlayerData.SetFloat("skill_rating", 1.0F);
            skillRatingDisplay.text = $"1.00";
        }

        if (PlayerData.HasKey(player, "wins"))
        {
            _wins = PlayerData.GetInt(player, "wins");
            wonDisplay.text = $"{_wins}";
        }
        else
        {
            PlayerData.SetInt("wins", 0);
            wonDisplay.text = $"0";
        }

        if (PlayerData.HasKey(player, "kills"))
        {
            _kills = PlayerData.GetInt(player, "kills");
            killDisplay.text = $"{_kills}";
        }
        else
        {
            PlayerData.SetInt("kills", 0);
            killDisplay.text = $"0";
        }

        if (PlayerData.HasKey(player, "points"))
        {
            _points = PlayerData.GetInt(player, "points");
            pointsDisplay.text = $"{_points}";
        }
        else
        {
            PlayerData.SetInt("points", 0);
            pointsDisplay.text = $"0";
        }
    }

    public void SetLevel(int level)
    {
        _level = level;
        levelDisplay.text = _level.ToString();
    }

    public void SetPosition(int newPosition)
    {
        rankDisplay.text = newPosition.ToString();
    }
    
    public VRCPlayerApi GetPlayer()
    {
        return Networking.GetOwner(gameObject);
    }

    public int GetLevel()
    {
        return _level;
    }

    public void OnDestroy()
    {
        statsManagers.SendCustomEventDelayedFrames(nameof(statsManagers.RefreshStats), 1);
    }
}
