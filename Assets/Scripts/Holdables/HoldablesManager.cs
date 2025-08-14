
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HoldablesManager : UdonSharpBehaviour
{
    [SerializeField] private ItemPlayerObjectController pItemPlayerObjectController;
    public ItemPlayerObjectController ItemPlayerObjectController => pItemPlayerObjectController;
    
    public void SetItemPlayerObjectController(ItemPlayerObjectController itemPlayerObjectController)
    {
        if (Utilities.IsValid(pItemPlayerObjectController))
        {
            pItemPlayerObjectController.Holdable.Pickup.Drop();
            pItemPlayerObjectController.SetItemActive(false);
        }

        pItemPlayerObjectController = itemPlayerObjectController;

        if (Utilities.IsValid(itemPlayerObjectController))
        {
            itemPlayerObjectController.SetItemActive(true);
        }
    }
}
