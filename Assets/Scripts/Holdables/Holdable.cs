
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Holdable : UdonSharpBehaviour
{
    [SerializeField] protected Application application;
    [SerializeField] protected Player player;
    [SerializeField] private ItemPlayerObjectController itemPlayerObjectController;
    [SerializeField] private VRC_Pickup pickup;
    [SerializeField] private Rigidbody rigidBody;
    
    private Slot _slot;
    protected ItemInstance _itemInstance;
    protected bool _isHolding;
    
    public VRC_Pickup Pickup => pickup;
    public Rigidbody Rigidbody => rigidBody;
    public Slot Slot => _slot;
    public ItemInstance ItemInstance => _itemInstance;

    public override void OnPickupUseUp()
    {
        if (_itemInstance.UseItem())
        {
            _slot.PlayerSlotPersistence.SaveItem(_itemInstance.Item, _itemInstance.Quantity);
        }
        else
        {
            _slot.PlayerSlotPersistence.DeleteItem();
            _slot.ResetSlot();
            pickup.Drop();
            itemPlayerObjectController.SetItemActive(false);
        }
        application.Player.SetUsingItem(false);
    }

    public override void OnPickup()
    {
        _isHolding = true;
        rigidBody.isKinematic = false;
    }

    public override void OnDrop()
    {
        _isHolding = false;
    }

    public void Init(ItemInstance itemInstance, Slot slot)
    {
        _itemInstance = itemInstance;
        _slot = slot;
    }
}
