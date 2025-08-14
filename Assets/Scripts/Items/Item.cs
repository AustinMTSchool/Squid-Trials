
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

    [Space, Header("VR Settings")] 
    [SerializeField] protected Mesh mesh;
    
    private ItemPlayerObjectController _itemPlayerObjectController;
    
    public string ID => id;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public int QuantityMax => quantityMax;
    public ItemPlayerObjectController ItemPlayerObjectController => _itemPlayerObjectController;
    public Mesh Mesh => mesh;
    
    public virtual void SetupSlot(Slot slot)
    {
        var item = slot.ItemInstance.Item;

        if (Networking.LocalPlayer.IsUserInVR())
        {
            SlotVR slotVR = (SlotVR) slot;
            if (slotVR != null)
            {
                slotVR.SetMesh(item.mesh);
                slotVR.SetInteractionDisplay("");
            }
        }
        else
        {
            SlotDesktop slotDesktop = (SlotDesktop) slot;
            if (slotDesktop != null)
            {
                slotDesktop.SetIcon(item.icon);
                slotDesktop.SetQuantity(slot.ItemInstance.Quantity);
                slotDesktop.SetNameDisplay(item.displayName);
            }
        }
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        var playerObjects = Networking.GetPlayerObjects(player);
        foreach (var obj in playerObjects)
        {
            Debug.Log("Looking for object " + obj.name + " searched");
            if (obj.name.Contains(id))
            {
                Debug.Log("FOUND");
                _itemPlayerObjectController = obj.GetComponent<ItemPlayerObjectController>();
            }
        }
    }

    public override string ToString()
    {
        return id;
    }
}
