
using System;
using UdonSharp;
using UdonSharp.Examples.Utilities;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SlotVR : Slot
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private string interactionDisplay;
    
    public MeshFilter MeshFilter => meshFilter;
    public string InteractionDisplay => interactionDisplay;

    private void Start()
    {
        _isVR = false;
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        var networkedObjects = Networking.GetPlayerObjects(player);
        foreach (var obj in networkedObjects)
        {
            var slotPersistence = obj.GetComponent<PlayerSlotPersistence>();
            if (slotPersistence != null)
            {
                if (slotPersistence.Index == slotIndex)
                {
                    _playerSlotPersistence = slotPersistence;
                    if (string.IsNullOrEmpty(_playerSlotPersistence.ItemID)) return;
                    Item item = application.ItemManager.GetItemByID(_playerSlotPersistence.ItemID);
                    int quantity = _playerSlotPersistence.Quantity;
                    SetItemSlot(item, quantity);
                }
            }
        }
    }

    public override void Interact()
    {
        Select();
    }

    public override void SetItemSlot(Item item, int quantity)
    {
        Debug.Log($"SetItemSlot of {name}");
        itemInstance.SetItem(item);
        itemInstance.SetQuantity(quantity);
        itemInstance.Item.SetupSlot(this);
        _hasItem = true;
    }

    public override bool AddItemSlot(Item item, int amount)
    {
        var total = itemInstance.Quantity + amount;
        if (total <= itemInstance.Item.QuantityMax)
        {
            itemInstance.SetItem(item);
            itemInstance.SetQuantity(total);
            itemInstance.Item.SetupSlot(this);
            return true;
        }
        return false;
    }
    
    public override void Select()
    {
        if (!_hasItem)
        {
            application.HoldablesManager.SetItemPlayerObjectController(null);
            return;
        }

        var headTrack = application.Player.VRCPlayerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 forward = headTrack.rotation * Vector3.forward;
        Vector3 position = headTrack.position + forward * 2F;
        Holdable holdable = itemInstance.Item.ItemPlayerObjectController.Holdable;
        holdable.transform.position = position;
        holdable.Rigidbody.isKinematic = true;
        holdable.Init(itemInstance, this);
        application.HoldablesManager.SetItemPlayerObjectController(itemInstance.Item.ItemPlayerObjectController);
    }

    public override void ResetSlot()
    {
        meshFilter.mesh = null;
        InteractionText = "";
        _hasItem = false;
    }

    public override void OnUseItem()
    {
        itemInstance.Item.SetupSlot(this);
    }
    
    public void SetMesh(Mesh mesh)
    {
        if (mesh == null)
        {
            Debug.LogError($"Attempted to set null mesh on {name}");
            return;
        }
        Debug.Log($"Mesh was set! {mesh.name}");
        meshFilter.mesh = mesh;
    }

    public void SetInteractionDisplay(string display)
    {
        InteractionText = display;
    }
}
