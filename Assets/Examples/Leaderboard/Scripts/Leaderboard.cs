using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

public class Leaderboard : UdonSharpBehaviour
{
    [Header("Variable settings")] [Tooltip("The key name for the value you want to use from PlayerData.")] [SerializeField]
    private string leaderboardKey = "PersistentKeyName";

    [Tooltip("The amount of decimals to show in the leaderboard. 0 will show no decimals. 1 will show 1 decimal. 2 will show 2 decimals and so on.")] [SerializeField, Range(0, 3)]
    private int amountOfDecimals;

    [Header("References")] [Tooltip("The parent transform for the leaderboard slots. This is where the leaderboard slots will be instantiated. It should have vertical layout group.")] [SerializeField]
    private Transform leaderboardSlotsParent;

    private LeaderboardSlot[] _leaderboardSlots;

    // When PlayerData is updated, we want to update the leaderboard UI if the leaderboard key is changed
    public override void OnPlayerDataUpdated(VRCPlayerApi player, PlayerData.Info[] infos)
    {
        if (PlayerData.HasKey(player, leaderboardKey))
        {
            UpdateSlotPosition(player);
        }
    }

    private void UpdateSlotPosition(VRCPlayerApi player)
    {
        
        LeaderboardSlot leaderboardSlot = PersistenceUtilities.GetPlayerObjectComponent<LeaderboardSlot>(player);
        if (!Utilities.IsValid(leaderboardSlot))
        {
            Debug.LogError($"Couldn't find a LeaderboardSlot for {player.displayName}.");
            return;
        }

        // Update the score of the player
        float playerScore = GetLeaderboardScore(player);
        leaderboardSlot.SetScore(playerScore, amountOfDecimals);

        // Get the current position in hierarchy
        int siblingIndex = leaderboardSlot.transform.GetSiblingIndex();

        // Refresh the _leaderboardSlots array to make sure we have the correct order
        _leaderboardSlots = leaderboardSlotsParent.GetComponentsInChildren<LeaderboardSlot>(true);
        
        // Check if player should move up in rank
        if (siblingIndex > 0 && playerScore > _leaderboardSlots[siblingIndex - 1].GetScore())
        {
            // Move the player up in the hierarchy
            leaderboardSlot.transform.SetSiblingIndex(siblingIndex - 1);
            
            // Update the position of the leaderboard slot that was moved due to us moving our slot up
            _leaderboardSlots[siblingIndex].SetPosition(siblingIndex + 1);

            // Check again to see if the player should move up again
            UpdateSlotPosition(player);
            
        } // Check if player should move down in rank
        else if (siblingIndex + 1 < _leaderboardSlots.Length && playerScore <= _leaderboardSlots[siblingIndex + 1].GetScore())
        {
            // Move the player down in the hierarchy
            leaderboardSlot.transform.SetSiblingIndex(siblingIndex + 1);
            
            // Update the position of the leaderboard slot that was moved due to us moving our slot down
            _leaderboardSlots[siblingIndex].SetPosition(siblingIndex + 1);

            // Check again to see if the player should move down again
            UpdateSlotPosition(player);
        }
        else
        {
            // Set the current position on the leaderboard, doing + 1 since we want to show the position starting from 1
            leaderboardSlot.SetPosition(siblingIndex + 1);
        }
    }

    // Function to get the player score. It's a bit complicated since we first need to figure out what type the leaderboard key is and then use the correct method to get value.
    // We cast to float so we can use the same method to set the score in the leaderboard slot for all types.
    private float GetLeaderboardScore(VRCPlayerApi player)
    {
        if (!PlayerData.HasKey(player, leaderboardKey))
        {
            return 0;
        }

        Type type = PlayerData.GetType(player, leaderboardKey);

        if (type == typeof(float))
        {
            return PlayerData.GetFloat(player, leaderboardKey);
        }
        else if (type == typeof(int))
        {
            return PlayerData.GetInt(player, leaderboardKey);
        }
        else
        {
            Debug.LogError($"The leaderboard key {leaderboardKey} is not a valid type. {type} is not implemented. Please use either float or int.");
            return 0;
        }
    }

    // Sets up the slot in the leaderboard. This is called from the LeaderboardSlot script when it's created.
    public void SetupSlot(LeaderboardSlot slot)
    {
        slot.transform.parent = leaderboardSlotsParent;
        _leaderboardSlots = leaderboardSlotsParent.GetComponentsInChildren<LeaderboardSlot>(true);

        slot.SetPosition(_leaderboardSlots.Length);

        slot.SetScore(GetLeaderboardScore(slot.GetPlayer()), amountOfDecimals);

        UpdateSlotPosition(slot.GetPlayer());
    }

    // Refreshes the leaderboard. This is called from the LeaderboardSlot script when it's destroyed.
    public void RefreshLeaderboard()
    {
        _leaderboardSlots = leaderboardSlotsParent.GetComponentsInChildren<LeaderboardSlot>(true);

        foreach (var slot in _leaderboardSlots)
        {
            UpdateSlotPosition(slot.GetPlayer());
        }
    }
}