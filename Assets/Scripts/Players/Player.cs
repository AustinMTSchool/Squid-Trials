
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class Player : UdonSharpBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayersManager playersManager;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerWalk playerWalk;
    [SerializeField] private Transform healthReference;
    [SerializeField] private PlayerEffects playerEffects;
    [SerializeField] private PlayerOverlayEffects playerOverlayEffects;
    [SerializeField] private ClassesManager classesManager;
    [SerializeField] private Transform playerStatReference;
    
    private VRCPlayerApi _vrcPlayer;
    private Health _health;
    private PlayerStat _playerStat;
    private bool _isPlayerRegistered = false;
    private bool _isPersistenceRestored = false;
    private bool _isPlayerInQueue = false;
    private bool _isUsingItem = false;
    private bool _isInGames = false;
    private bool _IsPushed = false;
    
    public ClassesManager ClassesManager => classesManager;
    public PlayerOverlayEffects PlayerOverlayEffects => playerOverlayEffects;
    public PlayerInventory Inventory => inventory;
    public bool IsPersistenceRestored => _isPersistenceRestored;
    public bool IsUsingItem => _isUsingItem;
    public bool IsPlayerRegistered => _isPlayerRegistered;
    public bool IsPlayerInQueue => _isPlayerInQueue;
    public bool IsInGames => _isInGames;
    public VRCPlayerApi VRCPlayerApi => _vrcPlayer;
    public Health Health => _health;
    public PlayerEffects PlayerEffects => playerEffects;
    public bool IsPushed => _IsPushed;
    public PlayerStat PlayerStat => _playerStat;
    
    private void Start()
    {
        AssignLocalPlayer();
    }

    public void AssignLocalPlayer()
    {
        if (Networking.LocalPlayer == null)
        {
            SendCustomEventDelayedSeconds(nameof(AssignLocalPlayer), 0.1F);
            return;
        }
       
        _vrcPlayer = Networking.LocalPlayer;
        playersManager.AddPlayerToAll(_vrcPlayer);
        _isPlayerRegistered = true;
    }
    
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        Component comp = Networking.FindComponentInPlayerObjects(player, healthReference);
        _health = comp.GetComponent<Health>();
        
        Component stats = Networking.FindComponentInPlayerObjects(player, playerStatReference);
        _playerStat = stats.GetComponent<PlayerStat>();
    }

    public void Death()
    {
        if (_isInGames) _isInGames = false;
        Debug.Log("PLAYER HAD DIED");
        _playerStat.DeathStat.AddDeath(1);
    }
    
    public string GetDisplayName()
    {
        return _vrcPlayer.displayName;
    }
    
    public void SetUsingItem(bool value)
    {
        _isUsingItem = value;
    }

    public void SetPlayerInQueue(bool value)
    {
        _isPlayerInQueue = value;
    }

    public void SetInGames(bool value)
    {
        _isInGames = value;
    }

    public void _SetIsPushed(bool value)
    {
        _IsPushed = value;
    }
}
