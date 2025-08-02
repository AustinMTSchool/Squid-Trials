
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Application : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers playerStatsManagers;
    [SerializeField] private Player player;

    public PlayerStatsManagers PlayerStatsManagers => playerStatsManagers;
    public Player Player => player;

}
