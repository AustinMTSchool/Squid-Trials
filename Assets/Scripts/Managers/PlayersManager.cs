
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class PlayersManager : UdonSharpBehaviour
{
    [SerializeField] private DataDictionary playerByID = new DataDictionary();
    
    public DataDictionary PlayerByID => playerByID;

    public void AddPlayerToAll(VRCPlayerApi player)
    {
        if (player == null) return;
        
        playerByID.Add(player.playerId, new DataToken(player));
    }

    public void GetPlayerFromAll(int id)
    {
        if (playerByID.TryGetValue(id, out var player))
        {
            Debug.Log("player token type: " + player.TokenType);
        }
    }
}
