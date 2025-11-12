
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Reward : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI _display;
    [SerializeField] private int experience;
    [SerializeField] private int points;

    private void Awake()
    {
        _display = GetComponent<TextMeshProUGUI>();
    }

    public void _Init()
    {
        _display.text = $"Reward\n" +
                        $"- Experience: {experience}\n" +
                        $"- Points: {points}";
    }
    
    public void _GiveReward()
    {
        player.PlayerStat.LevelStat.AddExperience(experience);
        player.PlayerStat.PointStat.AddPoints(points);
    }
}
