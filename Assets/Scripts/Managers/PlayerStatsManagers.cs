
using System;
using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerStatsManagers : UdonSharpBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private Transform parentSlot;
    private PlayerStat[] _playerStats;

    public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
    {
        if (PlayerData.HasKey(player, key))
        {
            UpdateSlotPosition(player);
        }
    }

    private void UpdateSlotPosition(VRCPlayerApi player)
    {
        PlayerStat playerStat = PersistenceUtilities.GetPlayerObjectComponent<PlayerStat>(player);
        if (!Utilities.IsValid(playerStat))
        {
            Debug.LogError($"[{nameof(PlayerStatsManagers)}] could not find slot for player stats of {player.displayName}");
            return;
        }

        int playerLevel = GetPlayerLevel(player);
        playerStat.SetLevel(playerLevel);
        
        int siblingIndex = playerStat.transform.GetSiblingIndex();
        
        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);

        if (siblingIndex > 0 && playerLevel > _playerStats[siblingIndex - 1].GetLevel())
        {
            PlayerStat displacedPlayer = _playerStats[siblingIndex - 1];
            playerStat.transform.SetSiblingIndex(siblingIndex - 1);
            displacedPlayer.SetPosition(siblingIndex + 1);
            UpdateSlotPosition(player);
        }
        else if (siblingIndex + 1 < _playerStats.Length && playerLevel <= _playerStats[siblingIndex + 1].GetLevel())
        {
            PlayerStat displacedPlayer = _playerStats[siblingIndex + 1];
            playerStat.transform.SetSiblingIndex(siblingIndex + 1);
            displacedPlayer.SetPosition(siblingIndex + 1);
            UpdateSlotPosition(player);
        }
        else
        {
            playerStat.SetPosition(siblingIndex + 1);
        }
    }

    private int GetPlayerLevel(VRCPlayerApi player)
    {
        if (!PlayerData.HasKey(player, key))
        {
            return 1;
        }

        return PlayerData.GetInt(player, key);
    }

    public void SetupSlot(PlayerStat playerStat)
    {
        playerStat.transform.parent = parentSlot;
        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);
        playerStat.SetPosition(_playerStats.Length);
        playerStat.SetLevel(GetPlayerLevel(playerStat.GetPlayer()));
        UpdateSlotPosition(playerStat.GetPlayer());
    }

    public void RefreshStats()
    {
        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);
        foreach (var stat in _playerStats)
        {
            UpdateSlotPosition(stat.GetPlayer());
        }
    }
}
