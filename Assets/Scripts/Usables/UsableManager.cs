
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UsableManager : UdonSharpBehaviour
{
    private bool _isCurrentUsing;
    
    public bool IsCurrentlyUsing => _isCurrentUsing;

    public void SetUsing(bool value)
    {
        _isCurrentUsing = value;
    }
}
