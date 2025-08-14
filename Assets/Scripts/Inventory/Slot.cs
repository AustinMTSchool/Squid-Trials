
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

public class Slot : UdonSharpBehaviour
{
    [SerializeField] protected Application application;
    [SerializeField] protected ItemInstance itemInstance;
    [SerializeField] protected int slotIndex;
    protected PlayerSlotPersistence _playerSlotPersistence;
    protected bool _isVR;
    
    protected bool _selected;
    protected bool _hasItem;
    
    public ItemInstance ItemInstance => itemInstance;
    public int SlotIndex => slotIndex;
    public bool HasItem => _hasItem;
    public PlayerSlotPersistence PlayerSlotPersistence => _playerSlotPersistence;
    public bool IsVR => _isVR;

    public virtual void SetItemSlot(Item item, int quantity) { }

    public virtual bool AddItemSlot(Item item, int amount) => false;

    public virtual void Select() { }
    
    public virtual void ResetSlot() { }

    public virtual void OnUseItem() { }
}
