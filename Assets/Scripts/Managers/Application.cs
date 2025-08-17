
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Application : UdonSharpBehaviour
{
    [SerializeField] private PlayerStatsManagers playerStatsManagers;
    [SerializeField] private Player player;
    [SerializeField] private PlayersManager playersManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Resources resources;
    [SerializeField] private HoldablesManager holdablesManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private UsableManager usableManager;

    public PlayerStatsManagers PlayerStatsManagers => playerStatsManagers;
    public PlayersManager PlayersManager => playersManager;
    public GameManager GameManager => gameManager;
    public Player Player => player;
    public Resources Resources => resources;
    public HoldablesManager HoldablesManager => holdablesManager;
    public ItemManager ItemManager => itemManager;
    public UsableManager UsableManager => usableManager;

}
