
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Inventory : UdonSharpBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Slot[] slots;
    [SerializeField] private Application application;
    
    public Slot[] Slots => slots;

    private void Start()
    {
        if (Networking.LocalPlayer.IsUserInVR())
        {
            container.SetActive(false);
        }
    }

    public bool AddItemInstance(Item item, int quantity, out Slot foundSlot)
    {
        foundSlot = null;
        foreach (Slot slot in slots)
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

        foreach (Slot slot in slots)
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

    public Slot GetSlotByIndex(int index)
    {
        return slots[index-1];
    }
}
