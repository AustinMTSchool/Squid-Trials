
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
    
    public PlayerOverlayEffects PlayerOverlayEffects => playerOverlayEffects;
    public ClassesManager ClassesManager => classesManager;
    private VRCPlayerApi _vrcPlayer;
    private Health _health;
    private bool _isPlayerRegistered = false;
    private bool _isPersistenceRestored = false;
    private bool _isPlayerInQueue = false;
    private bool _isUsingItem = false;
    private bool _isInGames = false;
    private bool _IsPushed = false;
    
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
        
        if (!PlayerData.HasKey(player, "skill_rating"))
        { 
            PlayerData.SetFloat("skill_rating", 1.0F);
        }
        
        if (!PlayerData.HasKey(player, "level")) 
        { 
            PlayerData.SetInt("level", 1);
        }
        
        if (!PlayerData.HasKey(player, "wins")) 
        { 
            PlayerData.SetInt("wins", 0);
        }

        if (!PlayerData.HasKey(player, "kills"))
        {
            PlayerData.SetInt("kills", 0);
        }
        
        if (!PlayerData.HasKey(player, "points"))
        {
            PlayerData.SetInt("points", 0);
        }

        Component comp = Networking.FindComponentInPlayerObjects(player, healthReference);
        _health = comp.GetComponent<Health>();
    }

    public void Death()
    {
        if (_isInGames) _isInGames = false;
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
