
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ShopPurchase : UdonSharpBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int quantity;
    [SerializeField] private Application application;

    public void Purchase()
    {
        // Take points out
        if (application.Player.Inventory.AddItem(item, 1))
        {
            
        }
    }
}
