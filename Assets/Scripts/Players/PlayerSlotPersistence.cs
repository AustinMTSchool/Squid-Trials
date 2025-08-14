
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerSlotPersistence : UdonSharpBehaviour
{
    [SerializeField] private int index;
    [UdonSynced] private string _itemID;
    [UdonSynced] private int _quantity;
    
    public string ItemID => _itemID;
    public int Quantity => _quantity;
    
    public int Index => index;

    public void SaveItem(Item item, int quantity)
    {
        _itemID = item.ToString();
        _quantity = quantity;
        RequestSerialization();
    }

    public void DeleteItem()
    {
        if (_itemID == null) return;
        _itemID = "";
        _quantity = 0;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        
    }
}
