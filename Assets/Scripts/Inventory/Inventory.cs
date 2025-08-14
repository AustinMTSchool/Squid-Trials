
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Inventory : UdonSharpBehaviour
{
    [SerializeField] protected GameObject container;
    [SerializeField] protected Application application;

    public GameObject Container => container;
    
    protected virtual void Start() { }

    public virtual bool AddItemInstance(Item item, int quantity, out Slot foundSlot)
    {
        foundSlot = null;
        return false;
    }

    public virtual Slot GetSlotByIndex(int index)
    {
        return null;
    }

    public virtual void Restore(bool value) { }
}
