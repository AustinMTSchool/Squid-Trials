
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HoldablesManager : UdonSharpBehaviour
{
    [SerializeField] private Holdable pHoldable;
    public Holdable Holdable => pHoldable;
    
    public Holdable GetPlayerHoldable() => pHoldable;

    public void SetHoldable(Holdable holdable)
    {
        if (pHoldable != null)
        {
            Destroy(pHoldable.gameObject);
        }
        pHoldable = holdable;
    }
}
