
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayersJoinedQueue : UdonSharpBehaviour
{
    [SerializeField] private TextMeshProUGUI allPlayersText;
    [SerializeField] private TextMeshProUGUI playersReadyText;

    private int _amountOfPlayers;
    private int _playersReady;
    
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        _amountOfPlayers = VRCPlayerApi.GetPlayerCount();
        UpdatePlayerCount();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        _amountOfPlayers = VRCPlayerApi.GetPlayerCount();
        UpdatePlayerCount();
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log("Player entered trigger");
        _playersReady++;
        UpdatePlayerReadyCount();
    }

    public override void OnPlayerCollisionEnter(VRCPlayerApi player)
    {
        Debug.Log("Player collision");
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        Debug.Log("Player left trigger");
        _playersReady--;
        UpdatePlayerReadyCount();
    }

    private void UpdatePlayerCount()
    {
        allPlayersText.text = $"{_amountOfPlayers}";
    }

    private void UpdatePlayerReadyCount()
    {
        playersReadyText.text = $"{_playersReady}";
    }
}