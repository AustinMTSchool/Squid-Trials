
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InventoryDesktop : Inventory
{
    [SerializeField] private SlotDesktop[] slots;
    
    public SlotDesktop[] Slots => slots;

    public override bool AddItemInstance(Item item, int quantity, out Slot foundSlot)
    {
        foundSlot = null;
        foreach (SlotDesktop slot in slots)
        {
            if (slot.HasItem && slot.ItemInstance.Item.ID == item.ID)
            {
                if (slot.AddItemSlot(item, quantity))
                {
                    foundSlot = slot;
                    return true;
                }
            }
        }

        foreach (SlotDesktop slot in slots)
        {
            if (!slot.HasItem)
            {
                slot.SetItemSlot(item, quantity);
                foundSlot = slot;
                return true;
            }
            else
            {
                if (slot.ItemInstance.Item.ID == item.ID)
                {
                    if (slot.AddItemSlot(item, quantity))
                    {
                        foundSlot = slot;
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        return false;
        
    }

    public override Slot GetSlotByIndex(int index)
    {
        return slots[index-1];
    }
}
