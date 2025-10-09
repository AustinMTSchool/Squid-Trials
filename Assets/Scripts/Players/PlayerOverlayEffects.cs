
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerOverlayEffects : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float defaultDamageOpacity;
    [SerializeField] private int transitionTime = 5;
    
    private bool _isPlayerInVR;
    
    [Space, Header("Desktop")]
    [SerializeField] private GameObject canvasDesktop;
    [SerializeField] private Image damageOverlayDesktop;
    
    [Space, Header("VR")]
    [SerializeField] private GameObject canvasVR;
    [SerializeField] private Image damageOverlayVR;


    private void Start()
    {
        _isPlayerInVR = Networking.LocalPlayer.IsUserInVR();
    }

    public void _DamageOverlay(int health)
    {
        float multiplier = (float) player.Health.MaxHealthPoints / health;
        float opacity = multiplier * defaultDamageOpacity;
        
        if (_isPlayerInVR)
        {
            
        }
        else
        {
            damageOverlayDesktop.color = new Color(1, 1, 1, opacity);
            canvasDesktop.SetActive(true);
            damageOverlayDesktop.gameObject.SetActive(true);
            SendCustomEventDelayedSeconds(nameof(_EndOverlay), 5);
        }
        Debug.Log($"Opacity {opacity}");
    }

    public void _EndOverlay()
    {
        canvasDesktop.SetActive(false);
        damageOverlayDesktop.gameObject.SetActive(false);
    }

    public void _DamageTransition(float opacityMultiplier)
    {
        SendCustomEventDelayedSeconds(nameof(_DamageTransition), 0.05F);
    }
}
