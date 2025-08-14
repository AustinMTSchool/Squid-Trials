
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class InventoryVR : Inventory
{
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float updateRate = 0.02f;
    [SerializeField] private SlotVR[] slots;
    
    private Vector3 targetPosition;
    private float targetYRotation;
    private bool hasValidTarget = false;
    
    protected override void Start()
    {
        SendCustomEventDelayedFrames(nameof(UpdateInventoryVR), 1);
    }

    public void UpdateInventoryVR()
    {
        if (!Utilities.IsValid(application.Player.VRCPlayerApi))
        {
            SendCustomEventDelayedSeconds(nameof(UpdateInventoryVR), 0.05f);
            return;
        }
        
        var position = application.Player.VRCPlayerApi.GetBonePosition(HumanBodyBones.Hips);
        var quaternion = application.Player.VRCPlayerApi.GetBoneRotation(HumanBodyBones.Hips);
        
        targetPosition = position;
        targetYRotation = quaternion.eulerAngles.y;
        
        if (!hasValidTarget)
        {
            transform.position = targetPosition;
            Vector3 currentEuler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(currentEuler.x, targetYRotation, currentEuler.z);
            hasValidTarget = true;
        }
        else
        {
            float deltaTime = updateRate;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * deltaTime);
            Vector3 currentEuler = transform.rotation.eulerAngles;
            float currentYRotation = currentEuler.y;
            float newYRotation = Mathf.LerpAngle(currentYRotation, targetYRotation, smoothSpeed * deltaTime);
            transform.rotation = Quaternion.Euler(currentEuler.x, newYRotation, currentEuler.z);
        }
        SendCustomEventDelayedSeconds(nameof(UpdateInventoryVR), updateRate);
    }

    public override bool AddItemInstance(Item item, int quantity, out Slot foundSlot)
    {
        foundSlot = null;
        foreach (SlotVR slot in slots)
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

        foreach (SlotVR slot in slots)
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
        return slots[index - 1];
    }
}
