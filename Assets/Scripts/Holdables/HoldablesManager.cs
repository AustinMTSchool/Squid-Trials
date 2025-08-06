
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HoldablesManager : UdonSharpBehaviour
{
    [SerializeField] private Holdable holdable;
    public Holdable Holdable => holdable;
    
    public Holdable GetPlayerHoldable() => holdable;

    public void SetHoldable(Holdable holdable)
    {
        if (holdable != null)
        {
            Destroy(holdable.gameObject);
        }
        this.holdable = holdable;
    }
}
