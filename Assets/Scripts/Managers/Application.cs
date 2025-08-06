
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Application : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers playerStatsManagers;
    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Resources resources;
    [SerializeField] private HoldablesManager holdablesManager;

    public PlayerStatsManagers PlayerStatsManagers => playerStatsManagers;
    public GameManager GameManager => gameManager;
    public Player Player => player;
    public Resources Resources => resources;
    public HoldablesManager HoldablesManager => holdablesManager;

}
