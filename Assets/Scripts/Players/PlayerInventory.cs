
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerInventory : UdonSharpBehaviour
{
    private VRCPlayerApi _player;
    public Inventory Inventory;

    private int _currentSlotSelected = -1;

    private void Start()
    {
        OnKeySelectUpdate();
    }

    public void OnKeySelectUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha1) && _currentSlotSelected != 1)
        {
            Debug.Log("Selected 1");
            _currentSlotSelected = 1;
            Inventory.GetSlotByIndex(_currentSlotSelected).Select();
        }
        if (Input.GetKey(KeyCode.Alpha2) && _currentSlotSelected != 2)
        {
            Debug.Log("Selected 2");
            _currentSlotSelected = 2;
            Inventory.GetSlotByIndex(_currentSlotSelected).Select();
        }
        SendCustomEventDelayedSeconds(nameof(OnKeySelectUpdate), 0.05F);
    }
    
    public Slot GetSlotSelected()
    {
        return Inventory.Slots[_currentSlotSelected];
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (Inventory.AddItemInstance(item, quantity, out Slot slot))
        {
            return true;
        }

        return false;
    }
}