
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class LevelStat : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers manager;
    [SerializeField] private TextMeshProUGUI display;
    [SerializeField] private int[] allLevels;
    [SerializeField] private PointStat pointStat;

    [UdonSynced] private int _level = 1;
    [UdonSynced] private int _experience = 0;
    
    // (level, exp)
    private DataDictionary _allLevels = new DataDictionary();
    private int _maxLevel;
    

    public int Level => _level;
    public int Experience => _experience;

    private void Start()
    {
        for (int i = 0; i < allLevels.Length; i++)
        {
            int next = i + 1;
            string level = i + "";
            _allLevels.Add(next, allLevels[i]);
        }
        _maxLevel = allLevels.Length;
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.IsOwner(gameObject)) return;
        if (!player.isLocal) return;
        
        display.text = _level.ToString();
        if (_allLevels.TryGetValue(_level, TokenType.Int, out var value))
        {
            Debug.Log("You are level: " + _level + " you need " + value.Int + "exp");
        }
        RequestSerialization();
    }

    public void AddExperience(int amount)
    {
        if (_level == _maxLevel) return;
        
        _experience += amount;

        while (_allLevels.TryGetValue(_level, TokenType.Int, out var experienceTotalNeeded))
        {
            if (_experience >= experienceTotalNeeded.Int)
            {
                _experience -= experienceTotalNeeded.Int;
                AddLevel(1);
                pointStat.AddPoints(100 * _level);
            }
            else
            {
                break;
            }
        }
        
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
