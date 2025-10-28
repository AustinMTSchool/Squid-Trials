
using System;
using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.Persistence;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PlayerStatsManagers : UdonSharpBehaviour
{
    [SerializeField] private Transform parentSlot;
    [SerializeField] private PlayerStat playerStats;
    
    private PlayerStat[] _playerStats;
    private bool isInitialized = false;

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        isInitialized = true;
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(UpdateSlotPosition), Networking.LocalPlayer.playerId, playerStats.LevelStat.Level);
    }

    [NetworkCallable(40)]
    public void UpdateSlotPosition(int playerID, int level)
    {
        VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerID);
        Debug.Log("Updating Slot position");
        
        PlayerStat playerStat = PersistenceUtilities.GetPlayerObjectComponent<PlayerStat>(player);
        if (!Utilities.IsValid(playerStat))
        {
            Debug.LogError($"[{nameof(PlayerStatsManagers)}] could not find slot for player stats of {player.displayName}");
            return;
        }

        if (!isInitialized) return;

        int playerLevel = level;
        Debug.Log("player level: " + playerLevel);

        int siblingIndex = playerStat.transform.GetSiblingIndex();

        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);

        if (siblingIndex > 0 && playerLevel > _playerStats[siblingIndex - 1].LevelStat.Level)
        {
            Debug.Log("Moving Up");
            playerStat.transform.SetSiblingIndex(siblingIndex - 1);
            _playerStats[siblingIndex].SetPosition(siblingIndex + 1);
            UpdateSlotPosition(player.playerId, level);
        }
        else if (siblingIndex + 1 < _playerStats.Length && playerLevel <= _playerStats[siblingIndex + 1].LevelStat.Level)
        {
            Debug.Log("Moving Down");
            playerStat.transform.SetSiblingIndex(siblingIndex + 1);
            _playerStats[siblingIndex].SetPosition(siblingIndex + 1);
            UpdateSlotPosition(player.playerId, level);
        }
        else
        {
            UpdateAllRanks();
        }
    }
    
    private void UpdateAllRanks()
    {
        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);
        for (int i = 0; i < _playerStats.Length; i++)
        {
            _playerStats[i].SetPosition(i + 1);
        }
    }

    public void SetupSlot(PlayerStat playerStat)
    {
        playerStat.transform.parent = parentSlot;
        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);
        playerStat.SetPosition(_playerStats.Length);
        UpdateSlotPosition(playerStat.GetPlayer().playerId, playerStat.LevelStat.Level);
    }

    public void RefreshStats()
    {
        _playerStats = parentSlot.GetComponentsInChildren<PlayerStat>(true);

        foreach (var slot in _playerStats)
        {
            UpdateSlotPosition(slot.GetPlayer().playerId, slot.LevelStat.Level);
        }
    }
}
