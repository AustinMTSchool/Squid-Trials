
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Item : UdonSharpBehaviour
{
    [SerializeField] protected string id;
    [SerializeField] protected string displayName;
    [SerializeField, TextArea(15, 10)] protected string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private int quantityMax;
    [SerializeField] private Holdable holdable;
    
    public string ID => id;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public int QuantityMax => quantityMax;
    public Holdable Holdable => holdable;

    public virtual void SetupSlot(Slot slot)
    {
        var item = slot.ItemInstance.Item;
        slot.SetIcon(item.icon);
        slot.SetQuantity(slot.ItemInstance.Quantity);
        slot.SetNameDisplay(item.displayName);
    }
}
