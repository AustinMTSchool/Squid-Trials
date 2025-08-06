
using System.Text;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ItemInstance : UdonSharpBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity;
    
    public Item Item => item;
    public int Quantity => quantity;

    public bool UseItem()
    {
        if (quantity <= 1) return false;
        quantity--;
        return true;
    }
    
    public void SetItem(Item item)
    {
        item = item;
    }

    public void SetQuantity(int quantity)
    {
        quantity = quantity;
    }
}
