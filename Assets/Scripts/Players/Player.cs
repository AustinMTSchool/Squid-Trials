
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

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
    [SerializeField] private HudManager hudManager;
    [SerializeField] private Transform pushComp;
    
    
    private VRCPlayerApi _vrcPlayer;
    private Health _health;
    private PlayerStat _playerStat;
    private Push _push;
    private bool _isPlayerRegistered = false;
    private bool _isPersistenceRestored = false;
    private bool _isPlayerInQueue = false;
    private bool _isUsingItem = false;
    private bool _isInGames = false;
    private bool _IsPushed = false;
    private bool _isDead = false;
    
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
    public HudManager HudManager => hudManager;
    public Push Push => _push;
    public bool IsDead => _isDead;
    
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
        
        Component push = Networking.FindComponentInPlayerObjects(player, pushComp);
        _push = push.GetComponent<Push>();
    }

    public void Death()
    {
        if (_isInGames) _isInGames = false;
        _playerStat.DeathStat.AddDeath(1);

        if (_IsPushed)
        {
            Debug.Log("[Player] Pushed to death");
            if (_push.PushedBy != null)
            {
                Debug.Log("[Player] You were push killed by " + _push.PushedBy.displayName);
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(RewardKiller), _push.PushedBy.playerId);
            }
            else
            {
                Debug.Log("[Player] No killer found");
            }
        }
    }

    [NetworkCallable]
    public void RewardKiller(int killerID)
    {
        if (Networking.LocalPlayer.playerId != killerID) return;
        
        Debug.Log("[Player] Killed player");
        _playerStat.KillStat.AddKills(1);
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

    public void _SetIsDead(bool value)
    {
        _isDead = value;
    }
}
