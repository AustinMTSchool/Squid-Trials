
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class SlotDesktop : Slot
{
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI quantityDisplay;
    
    public Image Background => background;
    public Image Icon => icon;
    public TextMeshProUGUI NameDisplay => nameDisplay;
    public TextMeshProUGUI QuantityDisplay => quantityDisplay;

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

    public override void SetItemSlot(Item item, int quantity)
    {
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
        icon.sprite = application.Resources.BlankSprite;
        quantityDisplay.text = "";
        nameDisplay.text = "";
        _hasItem = false;
    }
    
    public override void OnUseItem()
    {
        itemInstance.Item.SetupSlot(this);
    }

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
    }

    public void SetNameDisplay(string name)
    {
        this.nameDisplay.text = name;
    }

    public void SetQuantity(int quantity)
    {
        quantityDisplay.text = $"x{quantity}";
    }
}
