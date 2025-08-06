
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Slot : UdonSharpBehaviour
{
    [SerializeField] private Application application;
    [SerializeField] private ItemInstance itemInstance;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI quantityDisplay;
    [SerializeField] private int slotIndex;

    private bool _selected;
    private bool _hasItem;
    
    public ItemInstance ItemInstance => itemInstance;
    public Image Background => background;
    public Image Icon => icon;
    public TextMeshProUGUI NameDisplay => nameDisplay;
    public TextMeshProUGUI QuantityDisplay => quantityDisplay;
    public int SlotIndex => slotIndex;
    public bool HasItem => _hasItem;

    public void SetItemSlot(Item item, int quantity)
    {
        itemInstance.SetItem(item);
        itemInstance.SetQuantity(quantity);
        itemInstance.Item.SetupSlot(this);
        _hasItem = true;
    }

    public bool AddItemSlot(Item item, int amount)
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

    public void Select()
    {
        if (!_hasItem) return;
        
        var head = application.Player.VRCPlayerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
        Vector3 forward = head.rotation * Vector3.forward;
        Vector3 position = head.position + Vector3.forward * 2F;
        GameObject holdableGameObject = Instantiate(itemInstance.Item.Holdable.gameObject);
        holdableGameObject.transform.position = position;
        holdableGameObject.GetComponent<Rigidbody>().isKinematic = true;
        var holdable = holdableGameObject.GetComponent<Holdable>();
        holdable.Init(itemInstance, this);
        application.HoldablesManager.SetHoldable(holdable);
    }

    public void ResetSlot()
    {
        icon.sprite = application.Resources.BlankSprite;
        quantityDisplay.text = "";
        nameDisplay.text = "";
        _hasItem = false;
    }

    public void OnUseItem()
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
