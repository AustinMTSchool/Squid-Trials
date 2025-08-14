
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerInventory : UdonSharpBehaviour
{
    [SerializeField] protected Player player;
    [SerializeField] private InventoryDesktop inventoryDesktop;
    [SerializeField] private InventoryVR inventoryVR;
    private Inventory _inventory;
    private VRCPlayerApi _player;
    private bool _initialized = false;
    private int _currentSlotSelected = -1;
    
    public bool Initialized => _initialized;


    private void Start()
    {
        SetInventory_Coroutine();
    }

    public void SetInventory_Coroutine()
    {
        if (!Utilities.IsValid(player.VRCPlayerApi))
        {
            SendCustomEventDelayedSeconds(nameof(SetInventory_Coroutine), 0.1F);
            return;
        }

        if (player.VRCPlayerApi.IsUserInVR())
        {
            _inventory = inventoryVR;
            inventoryDesktop.Container.SetActive(false);
        }
        else
        {
            _inventory = inventoryDesktop;
            inventoryVR.Container.SetActive(false);
        }
        
        _initialized = true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (player.IsUsingItem) return;
            _currentSlotSelected = 1;
            _inventory.GetSlotByIndex(_currentSlotSelected).Select();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (player.IsUsingItem) return;
            _currentSlotSelected = 2;
            _inventory.GetSlotByIndex(_currentSlotSelected).Select();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (player.IsUsingItem) return;
            _currentSlotSelected = 3;
            _inventory.GetSlotByIndex(_currentSlotSelected).Select();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (player.IsUsingItem) return;
            _currentSlotSelected = 4;
            _inventory.GetSlotByIndex(_currentSlotSelected).Select();
        }
    }

    public Slot GetSlotSelected()
    {
        if (_currentSlotSelected == -1) return null;
        return _inventory.GetSlotByIndex(_currentSlotSelected);
    }

    public bool AddItem(Item item, int quantity = 1)
    {
        if (_inventory.AddItemInstance(item, quantity, out Slot slot))
        {
            if (slot == null) return false;
            
            if (slot.PlayerSlotPersistence == null) Debug.Log("PlayerSlotPersistence null");
            slot.PlayerSlotPersistence.SaveItem(item, quantity);
            return true;
        }

        return false;
    }
}